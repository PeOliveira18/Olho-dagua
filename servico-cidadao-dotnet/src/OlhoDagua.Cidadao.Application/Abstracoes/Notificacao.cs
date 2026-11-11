using OlhoDagua.Cidadao.Domain.Enums;

namespace OlhoDagua.Cidadao.Application.Abstracoes;

public sealed record NotificacaoAlerta(
    int AlertaId,
    int AreaRiscoId,
    string Mensagem,
    Severidade Severidade,
    IReadOnlyList<Guid> Dispositivos);

public sealed record NotificacaoEnviada(
    Guid DispositivoId,
    int AlertaId,
    int AreaRiscoId,
    string Mensagem,
    Severidade Severidade,
    DateTimeOffset EnviadaEm);

public interface INotificador
{
    Task NotificarAsync(NotificacaoAlerta notificacao, CancellationToken ct = default);
}

public interface IHistoricoNotificacoes
{
    void Registrar(NotificacaoEnviada notificacao);
    IReadOnlyList<NotificacaoEnviada> Listar(Guid? dispositivoId, int limite);
}

public interface ISanitizadorPii
{
    string Sanitizar(string texto);
}
