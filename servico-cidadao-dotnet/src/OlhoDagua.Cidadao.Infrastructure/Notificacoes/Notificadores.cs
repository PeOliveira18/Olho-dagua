using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using OlhoDagua.Cidadao.Application.Abstracoes;

namespace OlhoDagua.Cidadao.Infrastructure.Notificacoes;

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
            await Task.Delay(5, ct);

            historico.Registrar(new NotificacaoEnviada(
                dispositivo, notificacao.AlertaId, notificacao.AreaRiscoId,
                notificacao.Mensagem, notificacao.Severidade, agora));
        }

        logger.LogInformation("Push simulado do alerta {AlertaId} enviado a {Quantidade} dispositivo(s).",
                              notificacao.AlertaId, notificacao.Dispositivos.Count);
    }
}

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
