using OlhoDagua.Cidadao.Domain.Comum;
using OlhoDagua.Cidadao.Domain.Excecoes;
using OlhoDagua.Cidadao.Domain.ValueObjects;

namespace OlhoDagua.Cidadao.Domain.Entidades;

/// <summary>
/// Local seguro (escola, ginásio, igreja) que recebe moradores durante um alagamento.
/// A ocupação só muda pelos métodos de entrada e saída, que protegem a capacidade.
/// </summary>
public class Abrigo : Entidade
{
    public int AreaRiscoId { get; private set; }
    public AreaRisco? AreaRisco { get; private set; }
    public string Nome { get; private set; } = null!;
    public string Endereco { get; private set; } = null!;
    public Coordenada Local { get; private set; } = null!;
    public int Capacidade { get; private set; }
    public int Ocupacao { get; private set; }
    public bool Acessivel { get; private set; }
    public bool AceitaAnimais { get; private set; }

    public int VagasLivres => Capacidade - Ocupacao;

    private Abrigo() { } // EF Core

    public Abrigo(int areaRiscoId, string nome, string endereco, Coordenada local, int capacidade,
                  bool acessivel, bool aceitaAnimais)
    {
        if (areaRiscoId <= 0) throw new DominioException("O abrigo precisa pertencer a uma área de risco.");
        if (string.IsNullOrWhiteSpace(nome)) throw new DominioException("O nome do abrigo é obrigatório.");
        if (string.IsNullOrWhiteSpace(endereco)) throw new DominioException("O endereço do abrigo é obrigatório.");
        if (capacidade <= 0) throw new DominioException("A capacidade do abrigo deve ser maior que zero.");

        AreaRiscoId = areaRiscoId;
        Nome = nome.Trim();
        Endereco = endereco.Trim();
        Local = local ?? throw new DominioException("A localização do abrigo é obrigatória.");
        Capacidade = capacidade;
        Acessivel = acessivel;
        AceitaAnimais = aceitaAnimais;
    }

    public bool TemVaga() => VagasLivres > 0;

    public void RegistrarEntrada(int pessoas)
    {
        if (pessoas <= 0) throw new DominioException("Informe pelo menos uma pessoa.");
        if (pessoas > VagasLivres) throw new AbrigoSemVagaException(Nome, VagasLivres, pessoas);

        Ocupacao += pessoas;
    }

    public void RegistrarSaida(int pessoas)
    {
        if (pessoas <= 0) throw new DominioException("Informe pelo menos uma pessoa.");
        if (pessoas > Ocupacao)
            throw new ConflitoDominioException($"O abrigo '{Nome}' tem só {Ocupacao} pessoa(s) registrada(s).");

        Ocupacao -= pessoas;
    }
}
