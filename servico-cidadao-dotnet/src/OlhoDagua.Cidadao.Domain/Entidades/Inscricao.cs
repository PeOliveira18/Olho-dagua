using OlhoDagua.Cidadao.Domain.Comum;
using OlhoDagua.Cidadao.Domain.Excecoes;

namespace OlhoDagua.Cidadao.Domain.Entidades;

/// <summary>
/// Um celular inscrito para receber os alertas de uma área.
/// De propósito, não guarda nome, telefone nem CPF: o dispositivo é
/// identificado só por um Guid aleatório gerado no app (privacidade / LGPD).
/// </summary>
public class Inscricao : Entidade
{
    public Guid DispositivoId { get; private set; }
    public int AreaRiscoId { get; private set; }
    public AreaRisco? AreaRisco { get; private set; }
    public bool PrefereFonteGrande { get; private set; }
    public bool PrefereAltoContraste { get; private set; }
    public DateTimeOffset CriadaEm { get; private set; }
    public DateTimeOffset? AtualizadaEm { get; private set; }

    private Inscricao() { } // EF Core

    public Inscricao(Guid dispositivoId, int areaRiscoId, bool prefereFonteGrande, bool prefereAltoContraste,
                     DateTimeOffset criadaEm)
    {
        if (dispositivoId == Guid.Empty) throw new DominioException("O identificador do dispositivo é obrigatório.");

        DispositivoId = dispositivoId;
        DefinirArea(areaRiscoId);
        PrefereFonteGrande = prefereFonteGrande;
        PrefereAltoContraste = prefereAltoContraste;
        CriadaEm = EmUtc(criadaEm);
    }

    /// <summary>O morador mudou de área ou de preferências de acessibilidade.</summary>
    public void Atualizar(int areaRiscoId, bool prefereFonteGrande, bool prefereAltoContraste, DateTimeOffset quando)
    {
        DefinirArea(areaRiscoId);
        PrefereFonteGrande = prefereFonteGrande;
        PrefereAltoContraste = prefereAltoContraste;
        AtualizadaEm = EmUtc(quando);
    }

    private void DefinirArea(int areaRiscoId)
    {
        if (areaRiscoId <= 0) throw new DominioException("A inscrição precisa de uma área de risco.");
        AreaRiscoId = areaRiscoId;
    }
}
