using Microsoft.AspNetCore.Mvc;
using OlhoDagua.Cidadao.Api.Infra;
using OlhoDagua.Cidadao.Application.Dtos;
using OlhoDagua.Cidadao.Application.Servicos;

namespace OlhoDagua.Cidadao.Api.Controllers;

/// <summary>Alertas de alagamento: consultados pelo app, criados pela Telemetria e pelo ML.</summary>
[ApiController]
[Route("api/alertas")]
[Produces("application/json")]
public sealed class AlertasController(IAlertaService servico) : ControllerBase
{
    /// <summary>Lista alertas, do mais prioritário para o menos prioritário.</summary>
    /// <param name="areaId">Filtra por área de risco.</param>
    /// <param name="ativos">true (padrão) traz só os alertas em vigor.</param>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<AlertaResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar([FromQuery] int? areaId, [FromQuery] bool ativos = true, CancellationToken ct = default) =>
        Ok(await servico.ListarAsync(areaId, ativos, ct));

    [HttpGet("{id:int}")]
    [ProducesResponseType<AlertaResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Obter(int id, CancellationToken ct) => Ok(await servico.ObterAsync(id, ct));

    /// <summary>Contrato ②: chamado pelo Serviço de Telemetria quando o nível do córrego sobe de faixa.</summary>
    [HttpPost("sensor")]
    [ExigeApiKey]
    [ProducesResponseType<AlertaCriadoResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CriarAlertaSensor(CriarAlertaSensorRequest request, CancellationToken ct)
    {
        var criado = await servico.CriarAlertaSensorAsync(request, ct);
        return CreatedAtAction(nameof(Obter), new { id = criado.AlertaId }, criado);
    }

    /// <summary>Chamado pelo modelo de ML com a probabilidade de chuva forte nas próximas horas.</summary>
    [HttpPost("previsao")]
    [ExigeApiKey]
    [ProducesResponseType<AlertaCriadoResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CriarAlertaPrevisao(CriarAlertaPrevisaoRequest request, CancellationToken ct)
    {
        var criado = await servico.CriarAlertaPrevisaoAsync(request, ct);
        return CreatedAtAction(nameof(Obter), new { id = criado.AlertaId }, criado);
    }

    /// <summary>Contrato ③: encerra o alerta quando o nível volta ao normal. Idempotente.</summary>
    [HttpPatch("{id:int}/encerrar")]
    [ExigeApiKey]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Encerrar(int id, CancellationToken ct)
    {
        await servico.EncerrarAsync(id, ct);
        return NoContent();
    }
}
