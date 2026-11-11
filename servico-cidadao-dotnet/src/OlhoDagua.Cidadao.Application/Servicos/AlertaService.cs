using OlhoDagua.Cidadao.Application.Abstracoes;
using OlhoDagua.Cidadao.Application.Dtos;
using OlhoDagua.Cidadao.Application.Excecoes;
using OlhoDagua.Cidadao.Application.Mapeamento;
using OlhoDagua.Cidadao.Domain.Entidades;
using OlhoDagua.Cidadao.Domain.Excecoes;

namespace OlhoDagua.Cidadao.Application.Servicos;

public sealed class AlertaService(
    IAlertaRepositorio alertas,
    IAreaRiscoRepositorio areas,
    IPublicadorAlertas publicador,
    TimeProvider relogio) : IAlertaService
{
    public async Task<IReadOnlyList<AlertaResponse>> ListarAsync(int? areaId, bool somenteAtivos, CancellationToken ct = default)
    {
        var lista = await alertas.ListarAsync(areaId, somenteAtivos, ct);

        return lista
            .OrderByDescending(a => a.Ativo)
            .ThenByDescending(a => a.CalcularPrioridade())
            .ThenByDescending(a => a.CriadoEm)
            .Select(a => a.ParaResponse(a.AreaRisco!))
            .ToList();
    }

    public async Task<AlertaResponse> ObterAsync(int id, CancellationToken ct = default)
    {
        var alerta = await alertas.ObterComAreaAsync(id, ct) ?? throw new RecursoNaoEncontradoException("Alerta", id);
        return alerta.ParaResponse(alerta.AreaRisco!);
    }

    public async Task<AlertaCriadoResponse> CriarAlertaSensorAsync(CriarAlertaSensorRequest request, CancellationToken ct = default)
    {
        var area = await ObterAreaOuFalharAsync(request.AreaRiscoId, ct);
        var detectadoEm = request.DetectadoEm ?? relogio.GetUtcNow();

        foreach (var anterior in await alertas.ListarSensorAtivosAsync(request.SensorCodigo, ct))
            anterior.Encerrar(detectadoEm > anterior.CriadoEm ? detectadoEm : anterior.CriadoEm);

        var alerta = new AlertaSensor(area.Id, request.SensorCodigo, request.NivelCm, request.Faixa!.Value, detectadoEm);

        return await SalvarEPublicarAsync(alerta, area, ct);
    }

    public async Task<AlertaCriadoResponse> CriarAlertaPrevisaoAsync(CriarAlertaPrevisaoRequest request, CancellationToken ct = default)
    {
        var area = await ObterAreaOuFalharAsync(request.AreaRiscoId, ct);
        var alerta = new AlertaPrevisao(area.Id, request.ProbabilidadeChuvaForte, request.JanelaHoras, relogio.GetUtcNow());

        return await SalvarEPublicarAsync(alerta, area, ct);
    }

    public async Task EncerrarAsync(int id, CancellationToken ct = default)
    {
        var alerta = await alertas.ObterPorIdAsync(id, ct) ?? throw new RecursoNaoEncontradoException("Alerta", id);
        if (!alerta.Ativo) return;

        var agora = relogio.GetUtcNow();
        alerta.Encerrar(agora > alerta.CriadoEm ? agora : alerta.CriadoEm);
        await alertas.SalvarAlteracoesAsync(ct);
    }

    private async Task<AlertaCriadoResponse> SalvarEPublicarAsync(Alerta alerta, AreaRisco area, CancellationToken ct)
    {
        await alertas.AdicionarAsync(alerta, ct);
        await alertas.SalvarAlteracoesAsync(ct);
        await publicador.PublicarAsync(alerta, area, ct);

        return new AlertaCriadoResponse(alerta.Id, alerta.GerarMensagem(area));
    }

    private async Task<AreaRisco> ObterAreaOuFalharAsync(int areaId, CancellationToken ct) =>
        await areas.ObterPorIdAsync(areaId, ct)
        ?? throw new DominioException($"A área de risco {areaId} não existe.");
}
