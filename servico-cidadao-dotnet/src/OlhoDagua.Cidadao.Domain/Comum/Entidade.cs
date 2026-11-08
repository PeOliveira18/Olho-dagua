namespace OlhoDagua.Cidadao.Domain.Comum;

/// <summary>
/// Base de todas as entidades do domínio: toda entidade tem uma identidade (Id)
/// atribuída pelo banco. O setter é protegido para que só o EF Core e as
/// próprias entidades possam alterá-lo.
/// </summary>
public abstract class Entidade
{
    public int Id { get; protected set; }

    /// <summary>
    /// O PostgreSQL (timestamptz) só aceita DateTimeOffset em UTC.
    /// Toda data que entra no domínio passa por aqui.
    /// </summary>
    protected static DateTimeOffset EmUtc(DateTimeOffset data) => data.ToUniversalTime();
}
