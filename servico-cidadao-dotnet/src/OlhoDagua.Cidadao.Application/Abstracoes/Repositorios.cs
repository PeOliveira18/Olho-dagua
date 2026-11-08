using OlhoDagua.Cidadao.Application.Dtos;
using OlhoDagua.Cidadao.Domain.Comum;
using OlhoDagua.Cidadao.Domain.Entidades;
using OlhoDagua.Cidadao.Domain.Enums;

namespace OlhoDagua.Cidadao.Application.Abstracoes;

/// <summary>
/// Operações comuns a qualquer repositório. A Application só conhece estas
/// interfaces; quem implementa (EF Core + PostgreSQL) é a Infrastructure.
/// </summary>
public interface IRepositorio<T> where T : Entidade
{
    Task<T?> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task AdicionarAsync(T entidade, CancellationToken ct = default);
    Task SalvarAlteracoesAsync(CancellationToken ct = default);
}

public interface IAreaRiscoRepositorio : IRepositorio<AreaRisco>
{
    Task<IReadOnlyList<AreaRisco>> ListarAsync(CancellationToken ct = default);
    Task<bool> ExisteAsync(int id, CancellationToken ct = default);
    Task<int> ContarAlertasAtivosAsync(int areaId, CancellationToken ct = default);
    Task<int> ContarOcorrenciasNaoResolvidasAsync(int areaId, CancellationToken ct = default);

    /// <summary>Áreas ordenadas pela quantidade de relatos desde uma data.</summary>
    Task<IReadOnlyList<RankingAreaResponse>> ObterRankingAsync(DateTimeOffset desde, CancellationToken ct = default);
}

public interface IAlertaRepositorio : IRepositorio<Alerta>
{
    /// <summary>Alertas com a área carregada (necessária para gerar a mensagem).</summary>
    Task<IReadOnlyList<Alerta>> ListarAsync(int? areaId, bool somenteAtivos, CancellationToken ct = default);
    Task<Alerta?> ObterComAreaAsync(int id, CancellationToken ct = default);
    Task<AlertaComunitario?> ObterComunitarioAtivoAsync(int areaId, CancellationToken ct = default);
    Task<IReadOnlyList<AlertaSensor>> ListarSensorAtivosAsync(string sensorCodigo, CancellationToken ct = default);
}

public interface IOcorrenciaRepositorio : IRepositorio<Ocorrencia>
{
    Task<IReadOnlyList<Ocorrencia>> ListarAsync(int? areaId, StatusOcorrencia? status, CancellationToken ct = default);
    Task<Ocorrencia?> ObterPorIdLocalAppAsync(Guid idLocalApp, CancellationToken ct = default);
    Task<IReadOnlyList<Ocorrencia>> ListarDaAreaDesdeAsync(int areaId, DateTimeOffset desde, CancellationToken ct = default);
}

public interface IAbrigoRepositorio : IRepositorio<Abrigo>
{
    Task<IReadOnlyList<Abrigo>> ListarAsync(int? areaId, bool? acessivel, bool somenteComVaga, CancellationToken ct = default);
}

public interface IInscricaoRepositorio : IRepositorio<Inscricao>
{
    Task<Inscricao?> ObterPorDispositivoAsync(Guid dispositivoId, CancellationToken ct = default);
    Task<IReadOnlyList<Guid>> ListarDispositivosDaAreaAsync(int areaId, CancellationToken ct = default);
}
