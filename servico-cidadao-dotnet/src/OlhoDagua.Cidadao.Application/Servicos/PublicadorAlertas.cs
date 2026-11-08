using OlhoDagua.Cidadao.Application.Abstracoes;
using OlhoDagua.Cidadao.Domain.Entidades;

namespace OlhoDagua.Cidadao.Application.Servicos;

/// <summary>
/// Monta a notificação de um alerta e entrega pelo INotificador configurado.
/// Não sabe se o canal é console ou push: isso é decidido na injeção de dependência.
/// </summary>
public sealed class PublicadorAlertas(IInscricaoRepositorio inscricoes, INotificador notificador) : IPublicadorAlertas
{
    public async Task PublicarAsync(Alerta alerta, AreaRisco area, CancellationToken ct = default)
    {
        var dispositivos = await inscricoes.ListarDispositivosDaAreaAsync(area.Id, ct);

        var notificacao = new NotificacaoAlerta(
            alerta.Id, area.Id, alerta.GerarMensagem(area), alerta.Severidade, dispositivos);

        await notificador.NotificarAsync(notificacao, ct);
    }
}
