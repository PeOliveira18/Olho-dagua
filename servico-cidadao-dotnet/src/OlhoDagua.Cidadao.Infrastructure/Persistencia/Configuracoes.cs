using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OlhoDagua.Cidadao.Domain.Entidades;
using OlhoDagua.Cidadao.Domain.ValueObjects;

namespace OlhoDagua.Cidadao.Infrastructure.Persistencia;

internal static class ConfiguracaoComum
{
    public static void MapearCoordenada<T>(this EntityTypeBuilder<T> b, System.Linq.Expressions.Expression<Func<T, Coordenada?>> nav)
        where T : class
    {
        b.OwnsOne(nav, c =>
        {
            c.Property(p => p.Latitude).HasColumnName("Latitude").IsRequired();
            c.Property(p => p.Longitude).HasColumnName("Longitude").IsRequired();
        });
        b.Navigation(nav).IsRequired();
    }
}

internal sealed class AreaRiscoConfig : IEntityTypeConfiguration<AreaRisco>
{
    public void Configure(EntityTypeBuilder<AreaRisco> b)
    {
        b.ToTable("OD_AREAS_RISCO");
        b.HasKey(a => a.Id);
        b.Property(a => a.Nome).HasMaxLength(120).IsRequired();
        b.Property(a => a.Bairro).HasMaxLength(120).IsRequired();
        b.MapearCoordenada(a => a.Centro);

        b.HasMany(a => a.Alertas).WithOne(x => x.AreaRisco).HasForeignKey(x => x.AreaRiscoId).OnDelete(DeleteBehavior.Restrict);
        b.HasMany(a => a.Ocorrencias).WithOne(x => x.AreaRisco).HasForeignKey(x => x.AreaRiscoId).OnDelete(DeleteBehavior.Restrict);
        b.HasMany(a => a.Abrigos).WithOne(x => x.AreaRisco).HasForeignKey(x => x.AreaRiscoId).OnDelete(DeleteBehavior.Restrict);
        b.Navigation(a => a.Alertas).UsePropertyAccessMode(PropertyAccessMode.Field);
        b.Navigation(a => a.Ocorrencias).UsePropertyAccessMode(PropertyAccessMode.Field);
        b.Navigation(a => a.Abrigos).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

internal sealed class AlertaConfig : IEntityTypeConfiguration<Alerta>
{
    public void Configure(EntityTypeBuilder<Alerta> b)
    {
        b.ToTable("OD_ALERTAS");
        b.HasKey(a => a.Id);

        b.HasDiscriminator<string>("TipoAlerta")
            .HasValue<AlertaSensor>("SENSOR")
            .HasValue<AlertaComunitario>("COMUNITARIO")
            .HasValue<AlertaPrevisao>("PREVISAO");
        b.Property<string>("TipoAlerta").HasMaxLength(20);

        b.Property(a => a.Severidade).HasConversion<string>().HasMaxLength(20);
        b.HasIndex(a => new { a.AreaRiscoId, a.EncerradoEm });
    }
}

internal sealed class AlertaSensorConfig : IEntityTypeConfiguration<AlertaSensor>
{
    public void Configure(EntityTypeBuilder<AlertaSensor> b)
    {
        b.Property(a => a.SensorCodigo).HasMaxLength(50);
        b.Property(a => a.Faixa).HasConversion<string>().HasMaxLength(20);
    }
}

internal sealed class OcorrenciaConfig : IEntityTypeConfiguration<Ocorrencia>
{
    public void Configure(EntityTypeBuilder<Ocorrencia> b)
    {
        b.ToTable("OD_OCORRENCIAS");
        b.HasKey(o => o.Id);
        b.Property(o => o.Tipo).HasConversion<string>().HasMaxLength(30);
        b.Property(o => o.Status).HasConversion<string>().HasMaxLength(20);
        b.Property(o => o.Descricao).HasMaxLength(Ocorrencia.TamanhoMaximoDescricao);
        b.MapearCoordenada(o => o.Local);
        b.HasIndex(o => o.IdLocalApp).IsUnique();
        b.HasIndex(o => new { o.AreaRiscoId, o.CriadaEm });
    }
}

internal sealed class AbrigoConfig : IEntityTypeConfiguration<Abrigo>
{
    public void Configure(EntityTypeBuilder<Abrigo> b)
    {
        b.ToTable("OD_ABRIGOS");
        b.HasKey(a => a.Id);
        b.Property(a => a.Nome).HasMaxLength(120).IsRequired();
        b.Property(a => a.Endereco).HasMaxLength(200).IsRequired();
        b.MapearCoordenada(a => a.Local);
        b.ToTable(t => t.HasCheckConstraint("CK_OD_ABRIGOS_OCUPACAO", "\"Ocupacao\" >= 0 AND \"Ocupacao\" <= \"Capacidade\""));
    }
}

internal sealed class InscricaoConfig : IEntityTypeConfiguration<Inscricao>
{
    public void Configure(EntityTypeBuilder<Inscricao> b)
    {
        b.ToTable("OD_INSCRICOES");
        b.HasKey(i => i.Id);
        b.HasIndex(i => i.DispositivoId).IsUnique();
        b.HasOne(i => i.AreaRisco).WithMany().HasForeignKey(i => i.AreaRiscoId).OnDelete(DeleteBehavior.Restrict);
    }
}
