using OlhoDagua.Cidadao.Domain.Enums;

namespace OlhoDagua.Cidadao.Application.Abstracoes;

/// <summary>O que é entregue a cada celular inscrito quando um alerta nasce.</summary>
public sealed record NotificacaoAlerta(
    int AlertaId,
    int AreaRiscoId,
    string Mensagem,
    Severidade Severidade,
    IReadOnlyList<Guid> Dispositivos);

/// <summary>Registro de um push que foi (simuladamente) enviado a um dispositivo.</summary>
public sealed record NotificacaoEnviada(
    Guid DispositivoId,
    int AlertaId,
    int AreaRiscoId,
    string Mensagem,
    Severidade Severidade,
    DateTimeOffset EnviadaEm);

/// <summary>
/// Canal de entrega dos alertas. Há mais de uma implementação (console, push
/// simulado) e a escolhida é injetada via DI conforme o appsettings.
/// </summary>
public interface INotificador
{
    Task NotificarAsync(NotificacaoAlerta notificacao, CancellationToken ct = default);
}

/// <summary>Histórico em memória dos pushes simulados, para a demonstração.</summary>
public interface IHistoricoNotificacoes
{
    void Registrar(NotificacaoEnviada notificacao);
    IReadOnlyList<NotificacaoEnviada> Listar(Guid? dispositivoId, int limite);
}

/// <summary>Remove ou mascara dados pessoais (CPF, telefone, e-mail) de textos livres.</summary>
public interface ISanitizadorPii
{
    string Sanitizar(string texto);
}
