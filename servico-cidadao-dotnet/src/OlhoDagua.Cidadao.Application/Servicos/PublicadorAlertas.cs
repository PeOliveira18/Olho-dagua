using OlhoDagua.Cidadao.Application.Abstracoes;
using OlhoDagua.Cidadao.Domain.Entidades;

namespace OlhoDagua.Cidadao.Application.Servicos;

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
