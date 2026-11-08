using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OlhoDagua.Cidadao.Application.Excecoes;
using OlhoDagua.Cidadao.Domain.Excecoes;

namespace OlhoDagua.Cidadao.Api.Infra;

/// <summary>
/// Converte exceções em respostas ProblemDetails (RFC 7807), o formato de erro
/// combinado entre os serviços. Regras de negócio violadas viram 400, conflitos
/// viram 409, recurso inexistente vira 404 e o resto vira 500 sem vazar detalhes.
/// </summary>
public sealed class TratadorDeErros(IProblemDetailsService problemDetails, ILogger<TratadorDeErros> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext http, Exception excecao, CancellationToken ct)
    {
        var (status, titulo) = excecao switch
        {
            RecursoNaoEncontradoException => (StatusCodes.Status404NotFound, "Recurso não encontrado"),
            ConflitoDominioException => (StatusCodes.Status409Conflict, "Conflito com o estado atual"),
            DominioException => (StatusCodes.Status400BadRequest, "Regra de negócio violada"),
            _ => (StatusCodes.Status500InternalServerError, "Erro interno")
        };

        if (status == StatusCodes.Status500InternalServerError)
            logger.LogError(excecao, "Erro não tratado em {Metodo} {Caminho}", http.Request.Method, http.Request.Path);

        http.Response.StatusCode = status;
        return await problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = http,
            Exception = excecao,
            ProblemDetails = new ProblemDetails
            {
                Status = status,
                Title = titulo,
                Detail = status == StatusCodes.Status500InternalServerError
                    ? "Ocorreu um erro inesperado. Tente novamente."
                    : excecao.Message
            }
        });
    }
}
