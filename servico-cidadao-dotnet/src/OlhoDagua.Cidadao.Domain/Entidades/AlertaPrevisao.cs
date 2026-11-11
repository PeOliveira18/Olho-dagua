using System.Globalization;
using OlhoDagua.Cidadao.Domain.Enums;
using OlhoDagua.Cidadao.Domain.Excecoes;

namespace OlhoDagua.Cidadao.Domain.Entidades;

public class AlertaPrevisao : Alerta
{
    public double ProbabilidadeChuvaForte { get; private set; }
    public int JanelaHoras { get; private set; }

    public override string Tipo => "PREVISAO";

    private AlertaPrevisao() { }

    public AlertaPrevisao(int areaRiscoId, double probabilidadeChuvaForte, int janelaHoras, DateTimeOffset criadoEm)
        : base(areaRiscoId, SeveridadeDaProbabilidade(probabilidadeChuvaForte), criadoEm)
    {
        if (janelaHoras is < 1 or > 24) throw new DominioException("A janela da previsão deve ser de 1 a 24 horas.");

        ProbabilidadeChuvaForte = probabilidadeChuvaForte;
        JanelaHoras = janelaHoras;
    }

    public static Severidade SeveridadeDaProbabilidade(double probabilidade)
    {
        if (probabilidade is < 0 or > 1)
            throw new DominioException("A probabilidade deve estar entre 0 e 1.");

        return probabilidade switch
        {
            >= 0.8 => Severidade.Alta,
            >= 0.6 => Severidade.Media,
            _ => Severidade.Baixa
        };
    }

    public override string GerarMensagem(AreaRisco area)
    {
        var chance = (ProbabilidadeChuvaForte * 100).ToString("0", CultureInfo.InvariantCulture);
        return $"Previsão de chuva forte em {area.Nome} nas próximas {JanelaHoras} h (chance de {chance}%). " +
               "Deixe documentos e remédios em local alto.";
    }

    public override int CalcularPrioridade() => PesoDaSeveridade + (int)Math.Round(ProbabilidadeChuvaForte * 20);
}
