using OlhoDagua.Cidadao.Domain.Comum;
using OlhoDagua.Cidadao.Domain.Enums;
using OlhoDagua.Cidadao.Domain.Excecoes;
using OlhoDagua.Cidadao.Domain.ValueObjects;

namespace OlhoDagua.Cidadao.Domain.Entidades;

public class Ocorrencia : Entidade
{
    public const int TamanhoMaximoDescricao = 280;
    public const int ConfirmacoesParaConfirmar = 2;

    public int AreaRiscoId { get; private set; }
    public AreaRisco? AreaRisco { get; private set; }
    public TipoOcorrencia Tipo { get; private set; }
    public string? Descricao { get; private set; }
    public Coordenada Local { get; private set; } = null!;
    public StatusOcorrencia Status { get; private set; }
    public int Confirmacoes { get; private set; }
    public DateTimeOffset CriadaEm { get; private set; }
    public DateTimeOffset? ResolvidaEm { get; private set; }

    public Guid IdLocalApp { get; private set; }

    private Ocorrencia() { }

    public Ocorrencia(int areaRiscoId, TipoOcorrencia tipo, string? descricao, Coordenada local,
                      Guid idLocalApp, DateTimeOffset criadaEm)
    {
        if (areaRiscoId <= 0) throw new DominioException("A ocorrência precisa pertencer a uma área de risco.");
        if (idLocalApp == Guid.Empty) throw new DominioException("O identificador local do app é obrigatório.");

        var texto = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
        if (texto?.Length > TamanhoMaximoDescricao)
            throw new DominioException($"A descrição pode ter no máximo {TamanhoMaximoDescricao} caracteres.");

        AreaRiscoId = areaRiscoId;
        Tipo = tipo;
        Descricao = texto;
        Local = local ?? throw new DominioException("A localização é obrigatória.");
        IdLocalApp = idLocalApp;
        CriadaEm = EmUtc(criadaEm);
        Status = StatusOcorrencia.Aberta;
    }

    public void Confirmar()
    {
        if (Status == StatusOcorrencia.Resolvida)
            throw new ConflitoDominioException("Não é possível confirmar uma ocorrência já resolvida.");

        Confirmacoes++;
        if (Confirmacoes >= ConfirmacoesParaConfirmar)
            Status = StatusOcorrencia.Confirmada;
    }

    public void Resolver(DateTimeOffset quando)
    {
        if (Status == StatusOcorrencia.Resolvida)
            throw new ConflitoDominioException("A ocorrência já está resolvida.");

        Status = StatusOcorrencia.Resolvida;
        ResolvidaEm = EmUtc(quando);
    }

    public bool ContaParaAlertaComunitario(DateTimeOffset agora, TimeSpan janela) =>
        Status != StatusOcorrencia.Resolvida && CriadaEm >= EmUtc(agora) - janela;
}
