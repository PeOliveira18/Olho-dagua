using OlhoDagua.Cidadao.Domain.Enums;
using OlhoDagua.Cidadao.Domain.Excecoes;

namespace OlhoDagua.Cidadao.Domain.Entidades;

/// <summary>
/// Alerta disparado pelo Serviço de Telemetria quando o nível do córrego
/// medido por um sensor sobe de faixa.
/// </summary>
public class AlertaSensor : Alerta
{
    public string SensorCodigo { get; private set; } = null!;
    public double NivelCm { get; private set; }
    public FaixaNivel Faixa { get; private set; }

    public override string Tipo => "SENSOR";

    private AlertaSensor() { } // EF Core

    public AlertaSensor(int areaRiscoId, string sensorCodigo, double nivelCm, FaixaNivel faixa, DateTimeOffset detectadoEm)
        : base(areaRiscoId, SeveridadeDaFaixa(faixa), detectadoEm)
    {
        if (string.IsNullOrWhiteSpace(sensorCodigo)) throw new DominioException("O código do sensor é obrigatório.");
        if (nivelCm < 0) throw new DominioException("O nível da água não pode ser negativo.");

        SensorCodigo = sensorCodigo.Trim().ToUpperInvariant();
        NivelCm = nivelCm;
        Faixa = faixa;
    }

    /// <summary>A faixa NORMAL não gera alerta; as demais definem a severidade.</summary>
    public static Severidade SeveridadeDaFaixa(FaixaNivel faixa) => faixa switch
    {
        FaixaNivel.Atencao => Severidade.Media,
        FaixaNivel.Alerta => Severidade.Alta,
        FaixaNivel.Emergencia => Severidade.Critica,
        _ => throw new DominioException("A faixa NORMAL não gera alerta.")
    };

    public override string GerarMensagem(AreaRisco area) => Faixa switch
    {
        FaixaNivel.Atencao =>
            $"Córrego em ATENÇÃO em {area.Nome}: nível em {FormatarMetros(NivelCm)}. Fique atento e acompanhe pelo app.",
        FaixaNivel.Alerta =>
            $"Córrego em ALERTA em {area.Nome} ({FormatarMetros(NivelCm)}). Tire documentos e eletrônicos do chão e prepare-se para sair.",
        _ =>
            $"EMERGÊNCIA em {area.Nome}: córrego a {FormatarMetros(NivelCm)}. Saia agora e vá para o abrigo mais próximo."
    };

    /// <summary>Medição física é a fonte mais confiável: ganha o maior bônus.</summary>
    public override int CalcularPrioridade() => PesoDaSeveridade + 30;
}
