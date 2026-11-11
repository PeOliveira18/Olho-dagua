using OlhoDagua.Cidadao.Domain.Enums;
using OlhoDagua.Cidadao.Domain.Excecoes;

namespace OlhoDagua.Cidadao.Domain.Entidades;

public class AlertaComunitario : Alerta
{
    public int QuantidadeRelatos { get; private set; }

    public override string Tipo => "COMUNITARIO";

    private AlertaComunitario() { }

    public AlertaComunitario(int areaRiscoId, int quantidadeRelatos, DateTimeOffset criadoEm)
        : base(areaRiscoId, Severidade.Alta, criadoEm)
    {
        if (quantidadeRelatos < 1) throw new DominioException("Um alerta comunitário precisa de pelo menos um relato.");
        QuantidadeRelatos = quantidadeRelatos;
    }

    public void RegistrarNovoRelato()
    {
        if (!Ativo) throw new ConflitoDominioException("Não é possível reforçar um alerta encerrado.");
        QuantidadeRelatos++;
    }

    public override string GerarMensagem(AreaRisco area) =>
        $"{QuantidadeRelatos} moradores relataram alagamento em {area.Nome} na última hora. " +
        "Evite a região e proteja seus pertences.";

    public override int CalcularPrioridade() => PesoDaSeveridade + Math.Min(QuantidadeRelatos, 5) * 5;
}
