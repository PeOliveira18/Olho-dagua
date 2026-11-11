namespace OlhoDagua.Cidadao.Domain.Comum;

public abstract class Entidade
{
    public int Id { get; protected set; }

    protected static DateTimeOffset EmUtc(DateTimeOffset data) => data.ToUniversalTime();
}
