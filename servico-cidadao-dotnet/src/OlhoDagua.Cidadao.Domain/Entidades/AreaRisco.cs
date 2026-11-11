using OlhoDagua.Cidadao.Domain.Comum;
using OlhoDagua.Cidadao.Domain.Excecoes;
using OlhoDagua.Cidadao.Domain.ValueObjects;

namespace OlhoDagua.Cidadao.Domain.Entidades;

public class AreaRisco : Entidade
{
    private readonly List<Alerta> _alertas = [];
    private readonly List<Ocorrencia> _ocorrencias = [];
    private readonly List<Abrigo> _abrigos = [];

    public string Nome { get; private set; } = null!;
    public string Bairro { get; private set; } = null!;
    public Coordenada Centro { get; private set; } = null!;
    public int RaioMetros { get; private set; }

    public int NivelRiscoHistorico { get; private set; }

    public IReadOnlyCollection<Alerta> Alertas => _alertas.AsReadOnly();
    public IReadOnlyCollection<Ocorrencia> Ocorrencias => _ocorrencias.AsReadOnly();
    public IReadOnlyCollection<Abrigo> Abrigos => _abrigos.AsReadOnly();

    private AreaRisco() { }

    public AreaRisco(string nome, string bairro, Coordenada centro, int raioMetros, int nivelRiscoHistorico)
    {
        if (string.IsNullOrWhiteSpace(nome)) throw new DominioException("O nome da área é obrigatório.");
        if (string.IsNullOrWhiteSpace(bairro)) throw new DominioException("O bairro é obrigatório.");
        if (raioMetros <= 0) throw new DominioException("O raio da área deve ser maior que zero.");
        if (nivelRiscoHistorico is < 1 or > 5)
            throw new DominioException("O nível de risco histórico deve estar entre 1 e 5.");

        Nome = nome.Trim();
        Bairro = bairro.Trim();
        Centro = centro ?? throw new DominioException("O centro da área é obrigatório.");
        RaioMetros = raioMetros;
        NivelRiscoHistorico = nivelRiscoHistorico;
    }

    public bool Contem(Coordenada ponto) => Centro.DistanciaMetros(ponto) <= RaioMetros;
}
