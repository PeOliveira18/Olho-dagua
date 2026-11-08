using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OlhoDagua.Cidadao.Application.Abstracoes;
using OlhoDagua.Cidadao.Infrastructure.Notificacoes;
using OlhoDagua.Cidadao.Infrastructure.Persistencia;
using OlhoDagua.Cidadao.Infrastructure.Repositorios;

namespace OlhoDagua.Cidadao.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' não configurada.");

        services.AddDbContext<CidadaoDbContext>(opcoes => opcoes.UseNpgsql(connectionString));

        services.AddScoped<IAreaRiscoRepositorio, AreaRiscoRepositorio>();
        services.AddScoped<IAlertaRepositorio, AlertaRepositorio>();
        services.AddScoped<IOcorrenciaRepositorio, OcorrenciaRepositorio>();
        services.AddScoped<IAbrigoRepositorio, AbrigoRepositorio>();
        services.AddScoped<IInscricaoRepositorio, InscricaoRepositorio>();

        // Troca de implementação só pela configuração: "Console" ou "PushSimulado" (padrão).
        services.AddSingleton<IHistoricoNotificacoes, HistoricoNotificacoesEmMemoria>();
        var tipoNotificador = configuration["Notificacao:Tipo"] ?? "PushSimulado";
        if (tipoNotificador.Equals("Console", StringComparison.OrdinalIgnoreCase))
            services.AddScoped<INotificador, NotificadorConsole>();
        else
            services.AddScoped<INotificador, NotificadorPushSimulado>();

        return services;
    }
}
