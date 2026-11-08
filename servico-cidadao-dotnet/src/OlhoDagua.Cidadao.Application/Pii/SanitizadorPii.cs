using System.Text.RegularExpressions;
using OlhoDagua.Cidadao.Application.Abstracoes;

namespace OlhoDagua.Cidadao.Application.Pii;

/// <summary>
/// Mascara dados pessoais que o morador pode digitar sem perceber na descrição
/// de um relato, antes que o texto seja salvo e exibido para os vizinhos:
/// CPF → ***.***.***-09 · telefone → (11) *****-7777 · e-mail → m***@dominio.com
/// </summary>
public sealed partial class SanitizadorPii : ISanitizadorPii
{
    public string Sanitizar(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto)) return texto;

        // A ordem importa: CPF primeiro, para o telefone não "comer" parte de um CPF.
        var resultado = Cpf().Replace(texto, m => $"***.***.***-{m.Groups["dv"].Value}");
        resultado = Telefone().Replace(resultado, m => $"({m.Groups["ddd"].Value}) *****-{m.Groups["fim"].Value}");
        resultado = Email().Replace(resultado, m => $"{m.Groups["inicio"].Value}***@{m.Groups["dominio"].Value}");
        return resultado;
    }

    // 123.456.789-09 ou 12345678909
    [GeneratedRegex(@"\b\d{3}\.?\d{3}\.?\d{3}-?(?<dv>\d{2})\b")]
    private static partial Regex Cpf();

    // +55 (11) 98888-7777 · 11 98888 7777 · (11)3333-4444
    [GeneratedRegex(@"(?:\+?55[\s-]?)?\(?\b(?<ddd>\d{2})\)?[\s-]?9?\d{4}[\s-]?(?<fim>\d{4})\b")]
    private static partial Regex Telefone();

    [GeneratedRegex(@"\b(?<inicio>[\w.+-])[\w.+-]*@(?<dominio>[\w-]+(?:\.[\w-]+)+)\b")]
    private static partial Regex Email();
}
