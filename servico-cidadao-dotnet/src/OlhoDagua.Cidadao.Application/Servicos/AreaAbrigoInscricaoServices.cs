using OlhoDagua.Cidadao.Application.Abstracoes;
using OlhoDagua.Cidadao.Application.Dtos;
using OlhoDagua.Cidadao.Application.Excecoes;
using OlhoDagua.Cidadao.Application.Mapeamento;
using OlhoDagua.Cidadao.Domain.Entidades;
using OlhoDagua.Cidadao.Domain.Excecoes;
using OlhoDagua.Cidadao.Domain.ValueObjects;

namespace OlhoDagua.Cidadao.Application.Servicos;

public sealed class AreaService(IAreaRiscoRepositorio areas, TimeProvider relogio) : IAreaService
{
    public async Task<IReadOnlyList<AreaResponse>> ListarAsync(CancellationToken ct = default) =>
        (await areas.ListarAsync(ct)).Select(a => a.ParaResponse()).ToList();

    public async Task<AreaDetalheResponse> ObterAsync(int id, CancellationToken ct = default)
    {
        var area = await areas.ObterPorIdAsync(id, ct) ?? throw new RecursoNaoEncontradoException("Área de risco", id);

        var alertasAtivos = await areas.ContarAlertasAtivosAsync(id, ct);
        var ocorrenciasAbertas = await areas.ContarOcorrenciasNaoResolvidasAsync(id, ct);

        return new AreaDetalheResponse(area.Id, area.Nome, area.Bairro, area.Centro.Latitude, area.Centro.Longitude,
                                       area.RaioMetros, area.NivelRiscoHistorico, alertasAtivos, ocorrenciasAbertas);
    }

    public async Task<IReadOnlyList<RankingAreaResponse>> ObterRankingAsync(int dias, CancellationToken ct = default)
    {
        if (dias is < 1 or > 365) throw new DominioException("O período do ranking deve ser de 1 a 365 dias.");
        return await areas.ObterRankingAsync(relogio.GetUtcNow().AddDays(-dias), ct);
    }
}

public sealed class AbrigoService(IAbrigoRepositorio abrigos) : IAbrigoService
{
    public async Task<IReadOnlyList<AbrigoResponse>> ListarAsync(int? areaId, CancellationToken ct = default) =>
        (await abrigos.ListarAsync(areaId, acessivel: null, somenteComVaga: false, ct)).Select(a => a.ParaResponse()).ToList();

    public async Task<AbrigoResponse> ObterAsync(int id, CancellationToken ct = default) =>
        (await ObterOuFalharAsync(id, ct)).ParaResponse();

    public async Task<IReadOnlyList<AbrigoProximoResponse>> ListarProximosAsync(
        double latitude, double longitude, bool? acessivel, int limite, CancellationToken ct = default)
    {
        if (limite is < 1 or > 50) throw new DominioException("O limite deve ser de 1 a 50 abrigos.");
        var morador = new Coordenada(latitude, longitude);

        var candidatos = await abrigos.ListarAsync(areaId: null, acessivel, somenteComVaga: true, ct);

        return candidatos
            .Select(a => new { Abrigo = a, Distancia = a.Local.DistanciaMetros(morador) })
            .OrderBy(x => x.Distancia)
            .Take(limite)
            .Select(x => x.Abrigo.ParaProximoResponse(x.Distancia))
            .ToList();
    }

    public async Task<AbrigoResponse> RegistrarEntradaAsync(int id, int pessoas, CancellationToken ct = default)
    {
        var abrigo = await ObterOuFalharAsync(id, ct);
        abrigo.RegistrarEntrada(pessoas);
        await abrigos.SalvarAlteracoesAsync(ct);
        return abrigo.ParaResponse();
    }

    public async Task<AbrigoResponse> RegistrarSaidaAsync(int id, int pessoas, CancellationToken ct = default)
    {
        var abrigo = await ObterOuFalharAsync(id, ct);
        abrigo.RegistrarSaida(pessoas);
        await abrigos.SalvarAlteracoesAsync(ct);
        return abrigo.ParaResponse();
    }

    private async Task<Abrigo> ObterOuFalharAsync(int id, CancellationToken ct) =>
        await abrigos.ObterPorIdAsync(id, ct) ?? throw new RecursoNaoEncontradoException("Abrigo", id);
}

public sealed class InscricaoService(
    IInscricaoRepositorio inscricoes,
    IAreaRiscoRepositorio areas,
    TimeProvider relogio) : IInscricaoService
{
    public async Task<Resultado<InscricaoResponse>> SalvarAsync(SalvarInscricaoRequest request, CancellationToken ct = default)
    {
        if (!await areas.ExisteAsync(request.AreaRiscoId, ct))
            throw new DominioException($"A área de risco {request.AreaRiscoId} não existe.");

        var agora = relogio.GetUtcNow();
        var existente = await inscricoes.ObterPorDispositivoAsync(request.DispositivoId!.Value, ct);

        if (existente is not null)
        {
            existente.Atualizar(request.AreaRiscoId, request.PrefereFonteGrande, request.PrefereAltoContraste, agora);
            await inscricoes.SalvarAlteracoesAsync(ct);
            return new Resultado<InscricaoResponse>(existente.ParaResponse(), Criado: false);
        }

        var nova = new Inscricao(request.DispositivoId.Value, request.AreaRiscoId,
                                 request.PrefereFonteGrande, request.PrefereAltoContraste, agora);
        await inscricoes.AdicionarAsync(nova, ct);
        await inscricoes.SalvarAlteracoesAsync(ct);
        return new Resultado<InscricaoResponse>(nova.ParaResponse(), Criado: true);
    }

    public async Task<InscricaoResponse> ObterAsync(Guid dispositivoId, CancellationToken ct = default) =>
        (await inscricoes.ObterPorDispositivoAsync(dispositivoId, ct)
         ?? throw new RecursoNaoEncontradoException("Inscrição do dispositivo", dispositivoId)).ParaResponse();
}
