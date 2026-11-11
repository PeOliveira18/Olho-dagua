namespace OlhoDagua.Cidadao.Application.Excecoes;

public sealed class RecursoNaoEncontradoException(string recurso, object chave)
    : Exception($"{recurso} '{chave}' não encontrado(a).");
