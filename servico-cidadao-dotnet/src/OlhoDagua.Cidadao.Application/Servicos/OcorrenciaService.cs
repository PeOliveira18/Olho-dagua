using OlhoDagua.Cidadao.Application.Abstracoes;
using OlhoDagua.Cidadao.Application.Dtos;
using OlhoDagua.Cidadao.Application.Excecoes;
using OlhoDagua.Cidadao.Application.Mapeamento;
using OlhoDagua.Cidadao.Domain.Entidades;
using OlhoDagua.Cidadao.Domain.Enums;
using OlhoDagua.Cidadao.Domain.Excecoes;
using OlhoDagua.Cidadao.Domain.ValueObjects;

namespace OlhoDagua.Cidadao.Application.Servicos;

public sealed class OcorrenciaService(
    IOcorrenciaRepositorio ocorrencias,
    IAreaRiscoRepositorio areas,
    IRegraAlertaComunitario regraComunitaria,
    IPublicadorAlertas publicador,
    ISanitizadorPii sanitizador,
    TimeProvider relogio) : IOcorrenciaService
{
    public async Task<IReadOnlyList<OcorrenciaResponse>> ListarAsync(int? areaId, StatusOcorrencia? status, CancellationToken ct = default) =>
        (await ocorrencias.ListarAsync(areaId, status, ct)).Select(o => o.ParaResponse()).ToList();

    public async Task<OcorrenciaResponse> ObterAsync(int id, CancellationToken ct = default) =>
        (await ObterOuFalharAsync(id, ct)).ParaResponse();

    public async Task<Resultado<OcorrenciaResponse>> RegistrarAsync(RegistrarOcorrenciaRequest request, CancellationToken ct = default)
    {
        // Idempotência: o app reenvia relatos da fila offline; o mesmo idLocalApp nunca duplica.
        var existente = await ocorrencias.ObterPorIdLocalAppAsync(request.IdLocalApp!.Value, ct);
        if (existente is not null)
            return new Resultado<OcorrenciaResponse>(existente.ParaResponse(), Criado: false);

        var local = new Coordenada(request.Latitude!.Value, request.Longitude!.Value);
        var area = await DescobrirAreaAsync(request.AreaRiscoId, local, ct);

        var descricao = request.Descricao is null ? null : sanitizador.Sanitizar(request.Descricao);
        var ocorrencia = new Ocorrencia(area.Id, request.Tipo!.Value, descricao, local,
                                        request.IdLocalApp.Value, relogio.GetUtcNow());

        await ocorrencias.AdicionarAsync(ocorrencia, ct);
        await ocorrencias.SalvarAlteracoesAsync(ct);

        var resultadoRegra = await regraComunitaria.AvaliarAsync(area.Id, ct);
        if (resultadoRegra is { AlertaNovo: true, Alerta: not null })
            await publicador.PublicarAsync(resultadoRegra.Alerta, area, ct);

        return new Resultado<OcorrenciaResponse>(ocorrencia.ParaResponse(), Criado: true);
    }

    public async Task<OcorrenciaResponse> ConfirmarAsync(int id, CancellationToken ct = default)
    {
        var ocorrencia = await ObterOuFalharAsync(id, ct);
        ocorrencia.Confirmar();
        await ocorrencias.SalvarAlteracoesAsync(ct);
        return ocorrencia.ParaResponse();
    }

    public async Task ResolverAsync(int id, CancellationToken ct = default)
    {
        var ocorrencia = await ObterOuFalharAsync(id, ct);
        ocorrencia.Resolver(relogio.GetUtcNow());
        await ocorrencias.SalvarAlteracoesAsync(ct);
    }

    /// <summary>
    /// Se o app informou a área, valida; se não, descobre pela localização
    /// (a área monitorada mais próxima cujo raio contém o ponto).
    /// </summary>
    private async Task<AreaRisco> DescobrirAreaAsync(int? areaId, Coordenada local, CancellationToken ct)
    {
        if (areaId is not null)
            return await areas.ObterPorIdAsync(areaId.Value, ct)
                   ?? throw new DominioException($"A área de risco {areaId} não existe.");

        var todas = await areas.ListarAsync(ct);
        return todas
                   .Where(a => a.Contem(local))
                   .OrderBy(a => a.Centro.DistanciaMetros(local))
                   .FirstOrDefault()
               ?? throw new DominioException("A localização informada está fora das áreas monitoradas.");
    }

    private async Task<Ocorrencia> ObterOuFalharAsync(int id, CancellationToken ct) =>
        await ocorrencias.ObterPorIdAsync(id, ct) ?? throw new RecursoNaoEncontradoException("Ocorrência", id);
}
