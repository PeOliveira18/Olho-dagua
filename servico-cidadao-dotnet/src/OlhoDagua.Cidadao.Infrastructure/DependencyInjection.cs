using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OlhoDagua.Cidadao.Application.Abstracoes;
using OlhoDagua.Cidadao.Infrastructure.Notificacoes;
using OlhoDagua.Cidadao.Infrastructure.Persistencia;
using OlhoDagua.Cidadao.Infrastructure.Repositorios;
using Oracle.EntityFrameworkCore.Infrastructure;

namespace OlhoDagua.Cidadao.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                "Connection string 'Default' não configurada. Veja a seção \"Como rodar\" do README (dotnet user-secrets).");

        services.AddDbContext<CidadaoDbContext>(opcoes => opcoes.UseOracle(
            connectionString,
            oracle => oracle
                .UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion19)
                .MigrationsHistoryTable("OD_EF_MIGRATIONS_HISTORY")));

        services.AddScoped<IAreaRiscoRepositorio, AreaRiscoRepositorio>();
        services.AddScoped<IAlertaRepositorio, AlertaRepositorio>();
        services.AddScoped<IOcorrenciaRepositorio, OcorrenciaRepositorio>();
        services.AddScoped<IAbrigoRepositorio, AbrigoRepositorio>();
        services.AddScoped<IInscricaoRepositorio, InscricaoRepositorio>();

        services.AddSingleton<IHistoricoNotificacoes, HistoricoNotificacoesEmMemoria>();
        var tipoNotificador = configuration["Notificacao:Tipo"] ?? "PushSimulado";
        if (tipoNotificador.Equals("Console", StringComparison.OrdinalIgnoreCase))
            services.AddScoped<INotificador, NotificadorConsole>();
        else
            services.AddScoped<INotificador, NotificadorPushSimulado>();

        return services;
    }
}
