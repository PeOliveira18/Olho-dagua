using Microsoft.EntityFrameworkCore;
using OlhoDagua.Cidadao.Domain.Entidades;

namespace OlhoDagua.Cidadao.Infrastructure.Persistencia;

public sealed class CidadaoDbContext(DbContextOptions<CidadaoDbContext> options) : DbContext(options)
{
    public DbSet<AreaRisco> Areas => Set<AreaRisco>();
    public DbSet<Alerta> Alertas => Set<Alerta>();
    public DbSet<Ocorrencia> Ocorrencias => Set<Ocorrencia>();
    public DbSet<Abrigo> Abrigos => Set<Abrigo>();
    public DbSet<Inscricao> Inscricoes => Set<Inscricao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CidadaoDbContext).Assembly);
}
