using Microsoft.AspNetCore.Mvc;
using OlhoDagua.Cidadao.Application.Dtos;
using OlhoDagua.Cidadao.Application.Servicos;
using OlhoDagua.Cidadao.Domain.Enums;

namespace OlhoDagua.Cidadao.Api.Controllers;

[ApiController]
[Route("api/ocorrencias")]
[Produces("application/json")]
public sealed class OcorrenciasController(IOcorrenciaService servico) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Lista relatos (coordenadas aproximadas e descrição sem dados pessoais).")]
    [ProducesResponseType<IReadOnlyList<OcorrenciaResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar([FromQuery] int? areaId, [FromQuery] StatusOcorrencia? status, CancellationToken ct) =>
        Ok(await servico.ListarAsync(areaId, status, ct));

    [HttpGet("{id:int}")]
    [EndpointSummary("Detalhe de um relato.")]
    [ProducesResponseType<OcorrenciaResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Obter(int id, CancellationToken ct) => Ok(await servico.ObterAsync(id, ct));

    [HttpPost]
    [EndpointSummary("Registra um relato de alagamento.")]
    [EndpointDescription(
        "Idempotente por idLocalApp: reenviar o mesmo relato devolve 200 com o existente, em vez de duplicar. " +
        "Pode gerar um alerta comunitário.")]
    [ProducesResponseType<OcorrenciaResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<OcorrenciaResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Registrar(RegistrarOcorrenciaRequest request, CancellationToken ct)
    {
        var resultado = await servico.RegistrarAsync(request, ct);
        return resultado.Criado
            ? CreatedAtAction(nameof(Obter), new { id = resultado.Valor.Id }, resultado.Valor)
            : Ok(resultado.Valor);
    }

    [HttpPost("{id:int}/confirmar")]
    [EndpointSummary("Outro morador confirma o relato. Na 2ª confirmação ele passa a CONFIRMADA.")]
    [ProducesResponseType<OcorrenciaResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Confirmar(int id, CancellationToken ct) => Ok(await servico.ConfirmarAsync(id, ct));

    [HttpPatch("{id:int}/resolver")]
    [EndpointSummary("Marca o relato como resolvido.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Resolver(int id, CancellationToken ct)
    {
        await servico.ResolverAsync(id, ct);
        return NoContent();
    }
}
