using System.Globalization;
using OlhoDagua.Cidadao.Domain.Comum;
using OlhoDagua.Cidadao.Domain.Enums;
using OlhoDagua.Cidadao.Domain.Excecoes;

namespace OlhoDagua.Cidadao.Domain.Entidades;

public abstract class Alerta : Entidade
{
    public int AreaRiscoId { get; private set; }
    public AreaRisco? AreaRisco { get; private set; }
    public Severidade Severidade { get; protected set; }
    public DateTimeOffset CriadoEm { get; private set; }
    public DateTimeOffset? EncerradoEm { get; private set; }

    public bool Ativo => EncerradoEm is null;

    public abstract string Tipo { get; }

    protected Alerta() { }

    protected Alerta(int areaRiscoId, Severidade severidade, DateTimeOffset criadoEm)
    {
        if (areaRiscoId <= 0) throw new DominioException("O alerta precisa pertencer a uma área de risco.");

        AreaRiscoId = areaRiscoId;
        Severidade = severidade;
        CriadoEm = EmUtc(criadoEm);
    }

    public abstract string GerarMensagem(AreaRisco area);

    public abstract int CalcularPrioridade();

    public void Encerrar(DateTimeOffset quando)
    {
        if (!Ativo) throw new ConflitoDominioException($"O alerta {Id} já foi encerrado.");

        var emUtc = EmUtc(quando);
        if (emUtc < CriadoEm) throw new DominioException("O alerta não pode ser encerrado antes de ter sido criado.");

        EncerradoEm = emUtc;
    }

    protected int PesoDaSeveridade => (int)Severidade * 100;

    protected static string FormatarMetros(double centimetros) =>
        (centimetros / 100).ToString("0.00", CultureInfo.InvariantCulture).Replace('.', ',') + " m";
}
