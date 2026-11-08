using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using OlhoDagua.Cidadao.Application.Abstracoes;

namespace OlhoDagua.Cidadao.Infrastructure.Notificacoes;

/// <summary>Implementação 1: só escreve o alerta no log do servidor.</summary>
public sealed class NotificadorConsole(ILogger<NotificadorConsole> logger) : INotificador
{
    public Task NotificarAsync(NotificacaoAlerta notificacao, CancellationToken ct = default)
    {
        logger.LogInformation(
            "ALERTA {AlertaId} ({Severidade}) para {Quantidade} dispositivo(s) da área {AreaId}: {Mensagem}",
            notificacao.AlertaId, notificacao.Severidade, notificacao.Dispositivos.Count,
            notificacao.AreaRiscoId, notificacao.Mensagem);

        return Task.CompletedTask;
    }
}

/// <summary>
/// Implementação 2: simula o envio de push para cada celular inscrito e guarda
/// o histórico em memória (consultável em GET /api/notificacoes para a demonstração).
/// Num ambiente real, aqui entraria o Expo Push ou o Firebase Cloud Messaging.
/// </summary>
public sealed class NotificadorPushSimulado(
    IHistoricoNotificacoes historico,
    TimeProvider relogio,
    ILogger<NotificadorPushSimulado> logger) : INotificador
{
    public async Task NotificarAsync(NotificacaoAlerta notificacao, CancellationToken ct = default)
    {
        var agora = relogio.GetUtcNow();

        foreach (var dispositivo in notificacao.Dispositivos)
        {
            ct.ThrowIfCancellationRequested();
            await Task.Delay(5, ct); // simula a latência de rede de um provedor de push

            historico.Registrar(new NotificacaoEnviada(
                dispositivo, notificacao.AlertaId, notificacao.AreaRiscoId,
                notificacao.Mensagem, notificacao.Severidade, agora));
        }

        logger.LogInformation("Push simulado do alerta {AlertaId} enviado a {Quantidade} dispositivo(s).",
                              notificacao.AlertaId, notificacao.Dispositivos.Count);
    }
}

/// <summary>Guarda os últimos pushes simulados. Singleton e seguro para várias requisições ao mesmo tempo.</summary>
public sealed class HistoricoNotificacoesEmMemoria : IHistoricoNotificacoes
{
    private const int Capacidade = 500;
    private readonly ConcurrentQueue<NotificacaoEnviada> _fila = new();

    public void Registrar(NotificacaoEnviada notificacao)
    {
        _fila.Enqueue(notificacao);
        while (_fila.Count > Capacidade && _fila.TryDequeue(out _)) { }
    }

    public IReadOnlyList<NotificacaoEnviada> Listar(Guid? dispositivoId, int limite) =>
        _fila
            .Where(n => dispositivoId is null || n.DispositivoId == dispositivoId)
            .OrderByDescending(n => n.EnviadaEm)
            .Take(limite)
            .ToList();
}
