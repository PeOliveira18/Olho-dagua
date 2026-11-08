using System.Globalization;
using OlhoDagua.Cidadao.Domain.Comum;
using OlhoDagua.Cidadao.Domain.Enums;
using OlhoDagua.Cidadao.Domain.Excecoes;

namespace OlhoDagua.Cidadao.Domain.Entidades;

/// <summary>
/// Alerta enviado aos moradores de uma área. É abstrato: cada origem de alerta
/// (sensor, comunidade, previsão do modelo de ML) é uma subclasse que decide
/// sozinha como escrever a mensagem e qual é a sua prioridade (polimorfismo).
/// No banco, as três subclasses ficam numa tabela só (TPH).
/// </summary>
public abstract class Alerta : Entidade
{
    public int AreaRiscoId { get; private set; }
    public AreaRisco? AreaRisco { get; private set; }
    public Severidade Severidade { get; protected set; }
    public DateTimeOffset CriadoEm { get; private set; }
    public DateTimeOffset? EncerradoEm { get; private set; }

    public bool Ativo => EncerradoEm is null;

    /// <summary>Código da origem do alerta, exposto na API: SENSOR, COMUNITARIO ou PREVISAO.</summary>
    public abstract string Tipo { get; }

    protected Alerta() { } // EF Core

    protected Alerta(int areaRiscoId, Severidade severidade, DateTimeOffset criadoEm)
    {
        if (areaRiscoId <= 0) throw new DominioException("O alerta precisa pertencer a uma área de risco.");

        AreaRiscoId = areaRiscoId;
        Severidade = severidade;
        CriadoEm = EmUtc(criadoEm);
    }

    /// <summary>Texto curto, em linguagem simples, que o morador vê no celular.</summary>
    public abstract string GerarMensagem(AreaRisco area);

    /// <summary>Quanto maior, mais no topo da lista do app o alerta aparece.</summary>
    public abstract int CalcularPrioridade();

    public void Encerrar(DateTimeOffset quando)
    {
        if (!Ativo) throw new ConflitoDominioException($"O alerta {Id} já foi encerrado.");

        var emUtc = EmUtc(quando);
        if (emUtc < CriadoEm) throw new DominioException("O alerta não pode ser encerrado antes de ter sido criado.");

        EncerradoEm = emUtc;
    }

    /// <summary>Peso base comum a todas as subclasses: a severidade vale mais que qualquer ajuste fino.</summary>
    protected int PesoDaSeveridade => (int)Severidade * 100;

    /// <summary>Formata centímetros como metros no padrão brasileiro: 235 → "2,35 m".</summary>
    protected static string FormatarMetros(double centimetros) =>
        (centimetros / 100).ToString("0.00", CultureInfo.InvariantCulture).Replace('.', ',') + " m";
}
