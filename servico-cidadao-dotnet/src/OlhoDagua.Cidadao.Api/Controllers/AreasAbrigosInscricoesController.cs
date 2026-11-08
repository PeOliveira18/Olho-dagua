using Microsoft.AspNetCore.Mvc;
using OlhoDagua.Cidadao.Application.Abstracoes;
using OlhoDagua.Cidadao.Application.Dtos;
using OlhoDagua.Cidadao.Application.Servicos;

namespace OlhoDagua.Cidadao.Api.Controllers;

/// <summary>Áreas de risco monitoradas.</summary>
[ApiController]
[Route("api/areas")]
[Produces("application/json")]
public sealed class AreasController(IAreaService servico) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<AreaResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(CancellationToken ct) => Ok(await servico.ListarAsync(ct));

    /// <summary>Detalhe da área com o número de alertas ativos e de relatos em aberto.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType<AreaDetalheResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Obter(int id, CancellationToken ct) => Ok(await servico.ObterAsync(id, ct));

    /// <summary>Áreas ordenadas pela quantidade de relatos no período.</summary>
    /// <param name="dias">Tamanho do período, em dias (padrão 7).</param>
    [HttpGet("ranking")]
    [ProducesResponseType<IReadOnlyList<RankingAreaResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Ranking([FromQuery] int dias = 7, CancellationToken ct = default) =>
        Ok(await servico.ObterRankingAsync(dias, ct));
}

/// <summary>Abrigos para onde os moradores podem ir durante um alagamento.</summary>
[ApiController]
[Route("api/abrigos")]
[Produces("application/json")]
public sealed class AbrigosController(IAbrigoService servico) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<AbrigoResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar([FromQuery] int? areaId, CancellationToken ct) =>
        Ok(await servico.ListarAsync(areaId, ct));

    [HttpGet("{id:int}")]
    [ProducesResponseType<AbrigoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Obter(int id, CancellationToken ct) => Ok(await servico.ObterAsync(id, ct));

    /// <summary>Abrigos com vaga, do mais próximo para o mais distante do morador.</summary>
    [HttpGet("proximos")]
    [ProducesResponseType<IReadOnlyList<AbrigoProximoResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Proximos([FromQuery] double lat, [FromQuery] double lon, [FromQuery] bool? acessivel,
                                              [FromQuery] int limite = 5, CancellationToken ct = default) =>
        Ok(await servico.ListarProximosAsync(lat, lon, acessivel, limite, ct));

    /// <summary>Registra a chegada de pessoas. Responde 409 se não houver vagas suficientes.</summary>
    [HttpPost("{id:int}/entrada")]
    [ProducesResponseType<AbrigoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Entrada(int id, MovimentoAbrigoRequest request, CancellationToken ct) =>
        Ok(await servico.RegistrarEntradaAsync(id, request.Pessoas, ct));

    [HttpPost("{id:int}/saida")]
    [ProducesResponseType<AbrigoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Saida(int id, MovimentoAbrigoRequest request, CancellationToken ct) =>
        Ok(await servico.RegistrarSaidaAsync(id, request.Pessoas, ct));
}

/// <summary>Inscrição anônima de um celular para receber os alertas de uma área.</summary>
[ApiController]
[Route("api/inscricoes")]
[Produces("application/json")]
public sealed class InscricoesController(IInscricaoService servico) : ControllerBase
{
    /// <summary>Cria a inscrição (201) ou atualiza a do mesmo dispositivo (200).</summary>
    [HttpPost]
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
    [ProducesResponseType<InscricaoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Obter(Guid dispositivoId, CancellationToken ct) =>
        Ok(await servico.ObterAsync(dispositivoId, ct));
}

/// <summary>Histórico dos pushes simulados (para a demonstração do envio de alertas).</summary>
[ApiController]
[Route("api/notificacoes")]
[Produces("application/json")]
public sealed class NotificacoesController(IHistoricoNotificacoes historico) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<NotificacaoEnviada>>(StatusCodes.Status200OK)]
    public IActionResult Listar([FromQuery] Guid? dispositivoId, [FromQuery] int limite = 50) =>
        Ok(historico.Listar(dispositivoId, Math.Clamp(limite, 1, 200)));
}
