using System.Text.RegularExpressions;
using OlhoDagua.Cidadao.Application.Abstracoes;

namespace OlhoDagua.Cidadao.Application.Pii;

public sealed partial class SanitizadorPii : ISanitizadorPii
{
    public string Sanitizar(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto)) return texto;

        var resultado = Cpf().Replace(texto, m => $"***.***.***-{m.Groups["dv"].Value}");
        resultado = Telefone().Replace(resultado, m => $"({m.Groups["ddd"].Value}) *****-{m.Groups["fim"].Value}");
        resultado = Email().Replace(resultado, m => $"{m.Groups["inicio"].Value}***@{m.Groups["dominio"].Value}");
        return resultado;
    }

    [GeneratedRegex(@"\b\d{3}\.?\d{3}\.?\d{3}-?(?<dv>\d{2})\b")]
    private static partial Regex Cpf();

    [GeneratedRegex(@"(?:\+?55[\s-]?)?\(?\b(?<ddd>\d{2})\)?[\s-]?9?\d{4}[\s-]?(?<fim>\d{4})\b")]
    private static partial Regex Telefone();

    [GeneratedRegex(@"\b(?<inicio>[\w.+-])[\w.+-]*@(?<dominio>[\w-]+(?:\.[\w-]+)+)\b")]
    private static partial Regex Email();
}
