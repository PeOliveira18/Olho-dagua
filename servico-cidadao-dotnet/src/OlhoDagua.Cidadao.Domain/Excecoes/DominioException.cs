namespace OlhoDagua.Cidadao.Domain.Excecoes;

/// <summary>Violação de uma regra de negócio. A API responde 400.</summary>
public class DominioException(string mensagem) : Exception(mensagem);

/// <summary>A operação conflita com o estado atual do recurso. A API responde 409.</summary>
public class ConflitoDominioException(string mensagem) : DominioException(mensagem);

/// <summary>Tentativa de registrar mais pessoas do que as vagas livres de um abrigo.</summary>
public sealed class AbrigoSemVagaException(string nomeAbrigo, int vagasLivres, int pessoas)
    : ConflitoDominioException(
        $"O abrigo '{nomeAbrigo}' tem {vagasLivres} vaga(s) livre(s) e não comporta {pessoas} pessoa(s).");
