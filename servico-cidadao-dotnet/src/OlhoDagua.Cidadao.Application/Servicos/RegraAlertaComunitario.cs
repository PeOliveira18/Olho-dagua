using OlhoDagua.Cidadao.Application.Abstracoes;
using OlhoDagua.Cidadao.Domain.Entidades;

namespace OlhoDagua.Cidadao.Application.Servicos;

public sealed class RegraAlertaComunitario(
    IOcorrenciaRepositorio ocorrencias,
    IAlertaRepositorio alertas,
    TimeProvider relogio) : IRegraAlertaComunitario
{
    public const int MinimoDeRelatos = 3;
    public static readonly TimeSpan Janela = TimeSpan.FromMinutes(60);

    public async Task<ResultadoRegraComunitaria> AvaliarAsync(int areaId, CancellationToken ct = default)
    {
        var agora = relogio.GetUtcNow();

        var existente = await alertas.ObterComunitarioAtivoAsync(areaId, ct);
        if (existente is not null)
        {
            existente.RegistrarNovoRelato();
            await alertas.SalvarAlteracoesAsync(ct);
            return new ResultadoRegraComunitaria(existente, AlertaNovo: false);
        }

        var recentes = await ocorrencias.ListarDaAreaDesdeAsync(areaId, agora - Janela, ct);
        var quantidade = recentes.Count(o => o.ContaParaAlertaComunitario(agora, Janela));
        if (quantidade < MinimoDeRelatos)
            return new ResultadoRegraComunitaria(null, AlertaNovo: false);

        var novo = new AlertaComunitario(areaId, quantidade, agora);
        await alertas.AdicionarAsync(novo, ct);
        await alertas.SalvarAlteracoesAsync(ct);
        return new ResultadoRegraComunitaria(novo, AlertaNovo: true);
    }
}
