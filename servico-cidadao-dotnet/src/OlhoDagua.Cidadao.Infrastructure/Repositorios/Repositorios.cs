using Microsoft.EntityFrameworkCore;
using OlhoDagua.Cidadao.Application.Abstracoes;
using OlhoDagua.Cidadao.Application.Dtos;
using OlhoDagua.Cidadao.Domain.Comum;
using OlhoDagua.Cidadao.Domain.Entidades;
using OlhoDagua.Cidadao.Domain.Enums;
using OlhoDagua.Cidadao.Infrastructure.Persistencia;

namespace OlhoDagua.Cidadao.Infrastructure.Repositorios;

public abstract class RepositorioEf<T>(CidadaoDbContext db) : IRepositorio<T> where T : Entidade
{
    protected CidadaoDbContext Db { get; } = db;
    protected DbSet<T> Conjunto => Db.Set<T>();

    public virtual async Task<T?> ObterPorIdAsync(int id, CancellationToken ct = default) =>
        await Conjunto.FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task AdicionarAsync(T entidade, CancellationToken ct = default) =>
        await Conjunto.AddAsync(entidade, ct);

    public Task SalvarAlteracoesAsync(CancellationToken ct = default) => Db.SaveChangesAsync(ct);
}

public sealed class AreaRiscoRepositorio(CidadaoDbContext db) : RepositorioEf<AreaRisco>(db), IAreaRiscoRepositorio
{
    public async Task<IReadOnlyList<AreaRisco>> ListarAsync(CancellationToken ct = default) =>
        await Conjunto.AsNoTracking().OrderBy(a => a.Id).ToListAsync(ct);

    public Task<bool> ExisteAsync(int id, CancellationToken ct = default) =>
        Conjunto.AnyAsync(a => a.Id == id, ct);

    public Task<int> ContarAlertasAtivosAsync(int areaId, CancellationToken ct = default) =>
        Db.Alertas.CountAsync(a => a.AreaRiscoId == areaId && a.EncerradoEm == null, ct);

    public Task<int> ContarOcorrenciasNaoResolvidasAsync(int areaId, CancellationToken ct = default) =>
        Db.Ocorrencias.CountAsync(o => o.AreaRiscoId == areaId && o.Status != StatusOcorrencia.Resolvida, ct);

    public async Task<IReadOnlyList<RankingAreaResponse>> ObterRankingAsync(DateTimeOffset desde, CancellationToken ct = default)
    {
        var desdeUtc = desde.ToUniversalTime();

        var linhas = await Conjunto
            .AsNoTracking()
            .Select(a => new
            {
                a.Id,
                a.Nome,
                a.Bairro,
                a.NivelRiscoHistorico,
                Total = Db.Ocorrencias.Count(o => o.AreaRiscoId == a.Id && o.CriadaEm >= desdeUtc)
            })
            .OrderByDescending(x => x.Total)
            .ThenByDescending(x => x.NivelRiscoHistorico)
            .ToListAsync(ct);

        return linhas
            .Select(x => new RankingAreaResponse(x.Id, x.Nome, x.Bairro, x.NivelRiscoHistorico, x.Total))
            .ToList();
    }
}

public sealed class AlertaRepositorio(CidadaoDbContext db) : RepositorioEf<Alerta>(db), IAlertaRepositorio
{
    public async Task<IReadOnlyList<Alerta>> ListarAsync(int? areaId, bool somenteAtivos, CancellationToken ct = default)
    {
        IQueryable<Alerta> consulta = Conjunto.AsNoTracking().Include(a => a.AreaRisco);

        if (areaId is not null) consulta = consulta.Where(a => a.AreaRiscoId == areaId);
        if (somenteAtivos) consulta = consulta.Where(a => a.EncerradoEm == null);

        return await consulta.OrderByDescending(a => a.CriadoEm).Take(200).ToListAsync(ct);
    }

    public Task<Alerta?> ObterComAreaAsync(int id, CancellationToken ct = default) =>
        Conjunto.AsNoTracking().Include(a => a.AreaRisco).FirstOrDefaultAsync(a => a.Id == id, ct);

    public Task<AlertaComunitario?> ObterComunitarioAtivoAsync(int areaId, CancellationToken ct = default) =>
        Conjunto.OfType<AlertaComunitario>()
            .Where(a => a.AreaRiscoId == areaId && a.EncerradoEm == null)
            .OrderByDescending(a => a.CriadoEm)
            .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<AlertaSensor>> ListarSensorAtivosAsync(string sensorCodigo, CancellationToken ct = default)
    {
        var codigo = sensorCodigo.Trim().ToUpperInvariant();
        return await Conjunto.OfType<AlertaSensor>()
            .Where(a => a.SensorCodigo == codigo && a.EncerradoEm == null)
            .ToListAsync(ct);
    }
}

public sealed class OcorrenciaRepositorio(CidadaoDbContext db) : RepositorioEf<Ocorrencia>(db), IOcorrenciaRepositorio
{
    public async Task<IReadOnlyList<Ocorrencia>> ListarAsync(int? areaId, StatusOcorrencia? status, CancellationToken ct = default)
    {
        var consulta = Conjunto.AsNoTracking().AsQueryable();

        if (areaId is not null) consulta = consulta.Where(o => o.AreaRiscoId == areaId);
        if (status is not null) consulta = consulta.Where(o => o.Status == status);

        return await consulta.OrderByDescending(o => o.CriadaEm).Take(200).ToListAsync(ct);
    }

    public Task<Ocorrencia?> ObterPorIdLocalAppAsync(Guid idLocalApp, CancellationToken ct = default) =>
        Conjunto.FirstOrDefaultAsync(o => o.IdLocalApp == idLocalApp, ct);

    public async Task<IReadOnlyList<Ocorrencia>> ListarDaAreaDesdeAsync(int areaId, DateTimeOffset desde, CancellationToken ct = default)
    {
        var desdeUtc = desde.ToUniversalTime();
        return await Conjunto.AsNoTracking()
            .Where(o => o.AreaRiscoId == areaId && o.CriadaEm >= desdeUtc)
            .ToListAsync(ct);
    }
}

public sealed class AbrigoRepositorio(CidadaoDbContext db) : RepositorioEf<Abrigo>(db), IAbrigoRepositorio
{
    public async Task<IReadOnlyList<Abrigo>> ListarAsync(int? areaId, bool? acessivel, bool somenteComVaga, CancellationToken ct = default)
    {
        var consulta = Conjunto.AsNoTracking().AsQueryable();

        if (areaId is not null) consulta = consulta.Where(a => a.AreaRiscoId == areaId);
        if (acessivel is not null) consulta = consulta.Where(a => a.Acessivel == acessivel);
        if (somenteComVaga) consulta = consulta.Where(a => a.Ocupacao < a.Capacidade);

        return await consulta.OrderBy(a => a.Nome).ToListAsync(ct);
    }
}

public sealed class InscricaoRepositorio(CidadaoDbContext db) : RepositorioEf<Inscricao>(db), IInscricaoRepositorio
{
    public Task<Inscricao?> ObterPorDispositivoAsync(Guid dispositivoId, CancellationToken ct = default) =>
        Conjunto.FirstOrDefaultAsync(i => i.DispositivoId == dispositivoId, ct);

    public async Task<IReadOnlyList<Guid>> ListarDispositivosDaAreaAsync(int areaId, CancellationToken ct = default) =>
        await Conjunto.AsNoTracking()
            .Where(i => i.AreaRiscoId == areaId)
            .Select(i => i.DispositivoId)
            .ToListAsync(ct);
}
