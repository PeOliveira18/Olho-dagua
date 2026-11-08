using OlhoDagua.Cidadao.Application.Dtos;
using OlhoDagua.Cidadao.Domain.Entidades;
using OlhoDagua.Cidadao.Domain.Enums;

namespace OlhoDagua.Cidadao.Application.Servicos;

public interface IAreaService
{
    Task<IReadOnlyList<AreaResponse>> ListarAsync(CancellationToken ct = default);
    Task<AreaDetalheResponse> ObterAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<RankingAreaResponse>> ObterRankingAsync(int dias, CancellationToken ct = default);
}

public interface IAlertaService
{
    Task<IReadOnlyList<AlertaResponse>> ListarAsync(int? areaId, bool somenteAtivos, CancellationToken ct = default);
    Task<AlertaResponse> ObterAsync(int id, CancellationToken ct = default);
    Task<AlertaCriadoResponse> CriarAlertaSensorAsync(CriarAlertaSensorRequest request, CancellationToken ct = default);
    Task<AlertaCriadoResponse> CriarAlertaPrevisaoAsync(CriarAlertaPrevisaoRequest request, CancellationToken ct = default);
    Task EncerrarAsync(int id, CancellationToken ct = default);
}

public interface IOcorrenciaService
{
    Task<IReadOnlyList<OcorrenciaResponse>> ListarAsync(int? areaId, StatusOcorrencia? status, CancellationToken ct = default);
    Task<OcorrenciaResponse> ObterAsync(int id, CancellationToken ct = default);

    /// <summary>Cria o relato, ou devolve o existente se o mesmo idLocalApp já foi enviado.</summary>
    Task<Resultado<OcorrenciaResponse>> RegistrarAsync(RegistrarOcorrenciaRequest request, CancellationToken ct = default);
    Task<OcorrenciaResponse> ConfirmarAsync(int id, CancellationToken ct = default);
    Task ResolverAsync(int id, CancellationToken ct = default);
}

public interface IAbrigoService
{
    Task<IReadOnlyList<AbrigoResponse>> ListarAsync(int? areaId, CancellationToken ct = default);
    Task<AbrigoResponse> ObterAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<AbrigoProximoResponse>> ListarProximosAsync(double latitude, double longitude, bool? acessivel,
                                                                   int limite, CancellationToken ct = default);
    Task<AbrigoResponse> RegistrarEntradaAsync(int id, int pessoas, CancellationToken ct = default);
    Task<AbrigoResponse> RegistrarSaidaAsync(int id, int pessoas, CancellationToken ct = default);
}

public interface IInscricaoService
{
    Task<Resultado<InscricaoResponse>> SalvarAsync(SalvarInscricaoRequest request, CancellationToken ct = default);
    Task<InscricaoResponse> ObterAsync(Guid dispositivoId, CancellationToken ct = default);
}

/// <summary>Entrega um alerta recém-criado a todos os dispositivos inscritos na área.</summary>
public interface IPublicadorAlertas
{
    Task PublicarAsync(Alerta alerta, AreaRisco area, CancellationToken ct = default);
}

/// <summary>Resultado da avaliação da regra comunitária para uma área.</summary>
public sealed record ResultadoRegraComunitaria(AlertaComunitario? Alerta, bool AlertaNovo);

/// <summary>
/// Decide se os relatos recentes de uma área justificam um alerta comunitário.
/// Fica isolada numa interface própria para ser testada sozinha.
/// </summary>
public interface IRegraAlertaComunitario
{
    Task<ResultadoRegraComunitaria> AvaliarAsync(int areaId, CancellationToken ct = default);
}
