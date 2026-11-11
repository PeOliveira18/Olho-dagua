namespace OlhoDagua.Cidadao.Domain.Excecoes;

public class DominioException(string mensagem) : Exception(mensagem);

public class ConflitoDominioException(string mensagem) : DominioException(mensagem);

public sealed class AbrigoSemVagaException(string nomeAbrigo, int vagasLivres, int pessoas)
    : ConflitoDominioException(
        $"O abrigo '{nomeAbrigo}' tem {vagasLivres} vaga(s) livre(s) e não comporta {pessoas} pessoa(s).");
