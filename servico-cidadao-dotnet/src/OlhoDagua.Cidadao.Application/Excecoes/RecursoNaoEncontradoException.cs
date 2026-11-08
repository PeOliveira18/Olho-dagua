namespace OlhoDagua.Cidadao.Application.Excecoes;

/// <summary>O recurso pedido na rota não existe. A API responde 404.</summary>
public sealed class RecursoNaoEncontradoException(string recurso, object chave)
    : Exception($"{recurso} '{chave}' não encontrado(a).");
