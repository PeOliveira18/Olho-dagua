using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using OlhoDagua.Cidadao.Api.Infra;
using OlhoDagua.Cidadao.Application;
using OlhoDagua.Cidadao.Infrastructure;
using OlhoDagua.Cidadao.Infrastructure.Persistencia;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Camadas (injeção de dependência): Application (regras) + Infrastructure (EF Core, notificadores).
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services
    .AddControllers()
    .AddJsonOptions(o =>
        // Enums como texto em MAIÚSCULAS, igual ao contrato: "ALERTA", "RUA_ALAGADA"...
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseUpper)));

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<TratadorDeErros>();
builder.Services.AddOpenApi();

// O app Expo (web) e o celular chamam a API de outra origem.
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseCors();

app.MapOpenApi();
app.MapScalarApiReference(opcoes => opcoes.WithTitle("Olho d'Água — Serviço do Cidadão"));
app.MapGet("/", () => Results.Redirect("/scalar")).ExcludeFromDescription();

app.MapControllers();

// Em desenvolvimento, aplica as migrations e popula os dados de demonstração ao subir.
if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("Banco:MigrarAoIniciar"))
{
    using var escopo = app.Services.CreateScope();
    var db = escopo.ServiceProvider.GetRequiredService<CidadaoDbContext>();
    await db.Database.MigrateAsync();
    await DbSeeder.SeedAsync(db);
}

await app.RunAsync();
