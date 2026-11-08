using OlhoDagua.Cidadao.Domain.Enums;
using OlhoDagua.Cidadao.Domain.Excecoes;

namespace OlhoDagua.Cidadao.Domain.Entidades;

/// <summary>
/// Alerta gerado pela própria comunidade: nasce quando vários moradores
/// relatam alagamento na mesma área em pouco tempo.
/// </summary>
public class AlertaComunitario : Alerta
{
    public int QuantidadeRelatos { get; private set; }

    public override string Tipo => "COMUNITARIO";

    private AlertaComunitario() { } // EF Core

    public AlertaComunitario(int areaRiscoId, int quantidadeRelatos, DateTimeOffset criadoEm)
        : base(areaRiscoId, Severidade.Alta, criadoEm)
    {
        if (quantidadeRelatos < 1) throw new DominioException("Um alerta comunitário precisa de pelo menos um relato.");
        QuantidadeRelatos = quantidadeRelatos;
    }

    /// <summary>Com o alerta já ativo, novos relatos reforçam o alerta em vez de criar outro.</summary>
    public void RegistrarNovoRelato()
    {
        if (!Ativo) throw new ConflitoDominioException("Não é possível reforçar um alerta encerrado.");
        QuantidadeRelatos++;
    }

    public override string GerarMensagem(AreaRisco area) =>
        $"{QuantidadeRelatos} moradores relataram alagamento em {area.Nome} na última hora. " +
        "Evite a região e proteja seus pertences.";

    /// <summary>Quanto mais relatos, mais confiável: até +25 de bônus.</summary>
    public override int CalcularPrioridade() => PesoDaSeveridade + Math.Min(QuantidadeRelatos, 5) * 5;
}
