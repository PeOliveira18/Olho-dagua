using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace OlhoDagua.Cidadao.Api.Infra;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public sealed class ExigeApiKeyAttribute : Attribute, IAsyncActionFilter
{
    public const string Header = "X-Api-Key";

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var configuracao = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        var chaveEsperada = configuracao["Integracao:ApiKey"];
        var chaveRecebida = context.HttpContext.Request.Headers[Header].ToString();

        if (string.IsNullOrEmpty(chaveEsperada) || !IguaisEmTempoConstante(chaveEsperada, chaveRecebida))
        {
            context.Result = new ObjectResult(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Chave de integração ausente ou inválida.",
                Detail = $"Envie o header {Header} com a chave do serviço."
            })
            { StatusCode = StatusCodes.Status401Unauthorized };
            return;
        }

        await next();
    }

    private static bool IguaisEmTempoConstante(string a, string b) =>
        CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(a), Encoding.UTF8.GetBytes(b));
}
