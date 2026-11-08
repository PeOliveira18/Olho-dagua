using Microsoft.Extensions.DependencyInjection;
using OlhoDagua.Cidadao.Application.Abstracoes;
using OlhoDagua.Cidadao.Application.Pii;
using OlhoDagua.Cidadao.Application.Servicos;

namespace OlhoDagua.Cidadao.Application;

public static class DependencyInjection
{
    /// <summary>Registra os serviços de aplicação. Scoped = uma instância por requisição HTTP.</summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<ISanitizadorPii, SanitizadorPii>();

        services.AddScoped<IPublicadorAlertas, PublicadorAlertas>();
        services.AddScoped<IRegraAlertaComunitario, RegraAlertaComunitario>();

        services.AddScoped<IAreaService, AreaService>();
        services.AddScoped<IAlertaService, AlertaService>();
        services.AddScoped<IOcorrenciaService, OcorrenciaService>();
        services.AddScoped<IAbrigoService, AbrigoService>();
        services.AddScoped<IInscricaoService, InscricaoService>();

        return services;
    }
}
