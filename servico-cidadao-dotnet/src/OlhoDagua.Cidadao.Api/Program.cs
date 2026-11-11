using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using OlhoDagua.Cidadao.Api.Infra;
using OlhoDagua.Cidadao.Application;
using OlhoDagua.Cidadao.Infrastructure;
using OlhoDagua.Cidadao.Infrastructure.Persistencia;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services
    .AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseUpper)));
builder.Services.ConfigureHttpJsonOptions(o =>
    o.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseUpper)));

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<TratadorDeErros>();
builder.Services.AddOpenApi(opcoes => opcoes.AddDocumentTransformer((documento, _, _) =>
{
    documento.Info.Title = "Olho d'Água — Serviço do Cidadão";
    documento.Info.Description =
        "API do sistema de alerta comunitário de alagamentos Olho d'Água (Global Solution FIAP 2026.2 · CitizenTech). " +
        "Guarda as áreas de risco, os alertas (sensor do córrego, comunidade e previsão de chuva), os relatos dos moradores, " +
        "os abrigos e as inscrições anônimas dos celulares. As rotas marcadas como integração exigem o header X-Api-Key. " +
        "Todos os dados são simulados.";
    return Task.CompletedTask;
}));

builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseCors();

app.MapOpenApi();
app.MapScalarApiReference(opcoes => opcoes.WithTitle("Olho d'Água — Serviço do Cidadão"));
app.MapGet("/", () => Results.Redirect("/scalar")).ExcludeFromDescription();

app.MapControllers();

if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("Banco:MigrarAoIniciar"))
{
    using var escopo = app.Services.CreateScope();
    var db = escopo.ServiceProvider.GetRequiredService<CidadaoDbContext>();
    await db.Database.MigrateAsync();
    await DbSeeder.SeedAsync(db);
}

await app.RunAsync();
