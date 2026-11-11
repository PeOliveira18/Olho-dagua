using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using OlhoDagua.Cidadao.Application.Abstracoes;
using OlhoDagua.Cidadao.Application.Dtos;
using OlhoDagua.Cidadao.Application.Servicos;

namespace OlhoDagua.Cidadao.Api.Controllers;

[ApiController]
[Route("api/areas")]
[Produces("application/json")]
public sealed class AreasController(IAreaService servico) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Lista as áreas de risco monitoradas.")]
    [ProducesResponseType<IReadOnlyList<AreaResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(CancellationToken ct) => Ok(await servico.ListarAsync(ct));

    [HttpGet("{id:int}")]
    [EndpointSummary("Detalhe da área com o número de alertas ativos e de relatos em aberto.")]
    [ProducesResponseType<AreaDetalheResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Obter(int id, CancellationToken ct) => Ok(await servico.ObterAsync(id, ct));

    [HttpGet("ranking")]
    [EndpointSummary("Áreas ordenadas pela quantidade de relatos no período.")]
    [ProducesResponseType<IReadOnlyList<RankingAreaResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Ranking(
        [FromQuery, Description("Tamanho do período, em dias (padrão 7).")] int dias = 7,
        CancellationToken ct = default) =>
        Ok(await servico.ObterRankingAsync(dias, ct));
}

[ApiController]
[Route("api/abrigos")]
[Produces("application/json")]
public sealed class AbrigosController(IAbrigoService servico) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Lista os abrigos.")]
    [ProducesResponseType<IReadOnlyList<AbrigoResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar([FromQuery] int? areaId, CancellationToken ct) =>
        Ok(await servico.ListarAsync(areaId, ct));

    [HttpGet("{id:int}")]
    [EndpointSummary("Detalhe de um abrigo.")]
    [ProducesResponseType<AbrigoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Obter(int id, CancellationToken ct) => Ok(await servico.ObterAsync(id, ct));

    [HttpGet("proximos")]
    [EndpointSummary("Abrigos com vaga, do mais próximo para o mais distante do morador.")]
    [ProducesResponseType<IReadOnlyList<AbrigoProximoResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Proximos([FromQuery] double lat, [FromQuery] double lon, [FromQuery] bool? acessivel,
                                              [FromQuery] int limite = 5, CancellationToken ct = default) =>
        Ok(await servico.ListarProximosAsync(lat, lon, acessivel, limite, ct));

    [HttpPost("{id:int}/entrada")]
    [EndpointSummary("Registra a chegada de pessoas. Responde 409 se não houver vagas suficientes.")]
    [ProducesResponseType<AbrigoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Entrada(int id, MovimentoAbrigoRequest request, CancellationToken ct) =>
        Ok(await servico.RegistrarEntradaAsync(id, request.Pessoas, ct));

    [HttpPost("{id:int}/saida")]
    [EndpointSummary("Registra a saída de pessoas do abrigo.")]
    [ProducesResponseType<AbrigoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Saida(int id, MovimentoAbrigoRequest request, CancellationToken ct) =>
        Ok(await servico.RegistrarSaidaAsync(id, request.Pessoas, ct));
}

[ApiController]
[Route("api/inscricoes")]
[Produces("application/json")]
public sealed class InscricoesController(IInscricaoService servico) : ControllerBase
{
    [HttpPost]
    [EndpointSummary("Cria a inscrição anônima do celular (201) ou atualiza a do mesmo dispositivo (200).")]
    [ProducesResponseType<InscricaoResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<InscricaoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Salvar(SalvarInscricaoRequest request, CancellationToken ct)
    {
        var resultado = await servico.SalvarAsync(request, ct);
        return resultado.Criado
            ? CreatedAtAction(nameof(Obter), new { dispositivoId = resultado.Valor.DispositivoId }, resultado.Valor)
            : Ok(resultado.Valor);
    }

    [HttpGet("{dispositivoId:guid}")]
    [EndpointSummary("Consulta a inscrição de um dispositivo.")]
    [ProducesResponseType<InscricaoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Obter(Guid dispositivoId, CancellationToken ct) =>
        Ok(await servico.ObterAsync(dispositivoId, ct));
}

[ApiController]
[Route("api/notificacoes")]
[Produces("application/json")]
public sealed class NotificacoesController(IHistoricoNotificacoes historico) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Histórico dos pushes simulados (para a demonstração do envio de alertas).")]
    [ProducesResponseType<IReadOnlyList<NotificacaoEnviada>>(StatusCodes.Status200OK)]
    public IActionResult Listar([FromQuery] Guid? dispositivoId, [FromQuery] int limite = 50) =>
        Ok(historico.Listar(dispositivoId, Math.Clamp(limite, 1, 200)));
}
