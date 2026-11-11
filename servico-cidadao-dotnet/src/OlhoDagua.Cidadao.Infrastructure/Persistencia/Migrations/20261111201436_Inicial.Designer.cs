using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OlhoDagua.Cidadao.Infrastructure.Persistencia;
using Oracle.EntityFrameworkCore.Metadata;

#nullable disable

namespace OlhoDagua.Cidadao.Infrastructure.Persistencia.Migrations
{
    [DbContext(typeof(CidadaoDbContext))]
    [Migration("20261111201436_Inicial")]
    partial class Inicial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "10.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            OracleModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("OlhoDagua.Cidadao.Domain.Entidades.Abrigo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(10)");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("AceitaAnimais")
                        .HasColumnType("NUMBER(1)");

                    b.Property<bool>("Acessivel")
                        .HasColumnType("NUMBER(1)");

                    b.Property<int>("AreaRiscoId")
                        .HasColumnType("NUMBER(10)");

                    b.Property<int>("Capacidade")
                        .HasColumnType("NUMBER(10)");

                    b.Property<string>("Endereco")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("NVARCHAR2(200)");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(120)
                        .HasColumnType("NVARCHAR2(120)");

                    b.Property<int>("Ocupacao")
                        .HasColumnType("NUMBER(10)");

                    b.HasKey("Id");

                    b.HasIndex("AreaRiscoId");

                    b.ToTable("OD_ABRIGOS", null, t =>
                        {
                            t.HasCheckConstraint("CK_OD_ABRIGOS_OCUPACAO", "\"Ocupacao\" >= 0 AND \"Ocupacao\" <= \"Capacidade\"");
                        });
                });

            modelBuilder.Entity("OlhoDagua.Cidadao.Domain.Entidades.Alerta", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(10)");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AreaRiscoId")
                        .HasColumnType("NUMBER(10)");

                    b.Property<DateTimeOffset>("CriadoEm")
                        .HasColumnType("TIMESTAMP(7) WITH TIME ZONE");

                    b.Property<DateTimeOffset?>("EncerradoEm")
                        .HasColumnType("TIMESTAMP(7) WITH TIME ZONE");

                    b.Property<string>("Severidade")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("NVARCHAR2(20)");

                    b.Property<string>("TipoAlerta")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("NVARCHAR2(20)");

                    b.HasKey("Id");

                    b.HasIndex("AreaRiscoId", "EncerradoEm");

                    b.ToTable("OD_ALERTAS", (string)null);

                    b.HasDiscriminator<string>("TipoAlerta").HasValue("Alerta");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("OlhoDagua.Cidadao.Domain.Entidades.AreaRisco", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(10)");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Bairro")
                        .IsRequired()
                        .HasMaxLength(120)
                        .HasColumnType("NVARCHAR2(120)");

                    b.Property<int>("NivelRiscoHistorico")
                        .HasColumnType("NUMBER(10)");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(120)
                        .HasColumnType("NVARCHAR2(120)");

                    b.Property<int>("RaioMetros")
                        .HasColumnType("NUMBER(10)");

                    b.HasKey("Id");

                    b.ToTable("OD_AREAS_RISCO", (string)null);
                });

            modelBuilder.Entity("OlhoDagua.Cidadao.Domain.Entidades.Inscricao", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(10)");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AreaRiscoId")
                        .HasColumnType("NUMBER(10)");

                    b.Property<DateTimeOffset?>("AtualizadaEm")
                        .HasColumnType("TIMESTAMP(7) WITH TIME ZONE");

                    b.Property<DateTimeOffset>("CriadaEm")
                        .HasColumnType("TIMESTAMP(7) WITH TIME ZONE");

                    b.Property<Guid>("DispositivoId")
                        .HasColumnType("RAW(16)");

                    b.Property<bool>("PrefereAltoContraste")
                        .HasColumnType("NUMBER(1)");

                    b.Property<bool>("PrefereFonteGrande")
                        .HasColumnType("NUMBER(1)");

                    b.HasKey("Id");

                    b.HasIndex("AreaRiscoId");

                    b.HasIndex("DispositivoId")
                        .IsUnique();

                    b.ToTable("OD_INSCRICOES", (string)null);
                });

            modelBuilder.Entity("OlhoDagua.Cidadao.Domain.Entidades.Ocorrencia", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(10)");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AreaRiscoId")
                        .HasColumnType("NUMBER(10)");

                    b.Property<int>("Confirmacoes")
                        .HasColumnType("NUMBER(10)");

                    b.Property<DateTimeOffset>("CriadaEm")
                        .HasColumnType("TIMESTAMP(7) WITH TIME ZONE");

                    b.Property<string>("Descricao")
                        .HasMaxLength(280)
                        .HasColumnType("NVARCHAR2(280)");

                    b.Property<Guid>("IdLocalApp")
                        .HasColumnType("RAW(16)");

                    b.Property<DateTimeOffset?>("ResolvidaEm")
                        .HasColumnType("TIMESTAMP(7) WITH TIME ZONE");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("NVARCHAR2(20)");

                    b.Property<string>("Tipo")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("NVARCHAR2(30)");

                    b.HasKey("Id");

                    b.HasIndex("IdLocalApp")
                        .IsUnique();

                    b.HasIndex("AreaRiscoId", "CriadaEm");

                    b.ToTable("OD_OCORRENCIAS", (string)null);
                });

            modelBuilder.Entity("OlhoDagua.Cidadao.Domain.Entidades.AlertaComunitario", b =>
                {
                    b.HasBaseType("OlhoDagua.Cidadao.Domain.Entidades.Alerta");

                    b.Property<int>("QuantidadeRelatos")
                        .HasColumnType("NUMBER(10)");

                    b.HasDiscriminator().HasValue("COMUNITARIO");
                });

            modelBuilder.Entity("OlhoDagua.Cidadao.Domain.Entidades.AlertaPrevisao", b =>
                {
                    b.HasBaseType("OlhoDagua.Cidadao.Domain.Entidades.Alerta");

                    b.Property<int>("JanelaHoras")
                        .HasColumnType("NUMBER(10)");

                    b.Property<double>("ProbabilidadeChuvaForte")
                        .HasColumnType("BINARY_DOUBLE");

                    b.HasDiscriminator().HasValue("PREVISAO");
                });

            modelBuilder.Entity("OlhoDagua.Cidadao.Domain.Entidades.AlertaSensor", b =>
                {
                    b.HasBaseType("OlhoDagua.Cidadao.Domain.Entidades.Alerta");

                    b.Property<string>("Faixa")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("NVARCHAR2(20)");

                    b.Property<double>("NivelCm")
                        .HasColumnType("BINARY_DOUBLE");

                    b.Property<string>("SensorCodigo")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("NVARCHAR2(50)");

                    b.HasDiscriminator().HasValue("SENSOR");
                });

            modelBuilder.Entity("OlhoDagua.Cidadao.Domain.Entidades.Abrigo", b =>
                {
                    b.HasOne("OlhoDagua.Cidadao.Domain.Entidades.AreaRisco", "AreaRisco")
                        .WithMany("Abrigos")
                        .HasForeignKey("AreaRiscoId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.OwnsOne("OlhoDagua.Cidadao.Domain.ValueObjects.Coordenada", "Local", b1 =>
                        {
                            b1.Property<int>("AbrigoId")
                                .HasColumnType("NUMBER(10)");

                            b1.Property<double>("Latitude")
                                .HasColumnType("BINARY_DOUBLE")
                                .HasColumnName("Latitude");

                            b1.Property<double>("Longitude")
                                .HasColumnType("BINARY_DOUBLE")
                                .HasColumnName("Longitude");

                            b1.HasKey("AbrigoId");

                            b1.ToTable("OD_ABRIGOS");

                            b1.WithOwner()
                                .HasForeignKey("AbrigoId");
                        });

                    b.Navigation("AreaRisco");

                    b.Navigation("Local")
                        .IsRequired();
                });

            modelBuilder.Entity("OlhoDagua.Cidadao.Domain.Entidades.Alerta", b =>
                {
                    b.HasOne("OlhoDagua.Cidadao.Domain.Entidades.AreaRisco", "AreaRisco")
                        .WithMany("Alertas")
                        .HasForeignKey("AreaRiscoId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("AreaRisco");
                });

            modelBuilder.Entity("OlhoDagua.Cidadao.Domain.Entidades.AreaRisco", b =>
                {
                    b.OwnsOne("OlhoDagua.Cidadao.Domain.ValueObjects.Coordenada", "Centro", b1 =>
                        {
                            b1.Property<int>("AreaRiscoId")
                                .HasColumnType("NUMBER(10)");

                            b1.Property<double>("Latitude")
                                .HasColumnType("BINARY_DOUBLE")
                                .HasColumnName("Latitude");

                            b1.Property<double>("Longitude")
                                .HasColumnType("BINARY_DOUBLE")
                                .HasColumnName("Longitude");

                            b1.HasKey("AreaRiscoId");

                            b1.ToTable("OD_AREAS_RISCO");

                            b1.WithOwner()
                                .HasForeignKey("AreaRiscoId");
                        });

                    b.Navigation("Centro")
                        .IsRequired();
                });

            modelBuilder.Entity("OlhoDagua.Cidadao.Domain.Entidades.Inscricao", b =>
                {
                    b.HasOne("OlhoDagua.Cidadao.Domain.Entidades.AreaRisco", "AreaRisco")
                        .WithMany()
                        .HasForeignKey("AreaRiscoId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("AreaRisco");
                });

            modelBuilder.Entity("OlhoDagua.Cidadao.Domain.Entidades.Ocorrencia", b =>
                {
                    b.HasOne("OlhoDagua.Cidadao.Domain.Entidades.AreaRisco", "AreaRisco")
                        .WithMany("Ocorrencias")
                        .HasForeignKey("AreaRiscoId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.OwnsOne("OlhoDagua.Cidadao.Domain.ValueObjects.Coordenada", "Local", b1 =>
                        {
                            b1.Property<int>("OcorrenciaId")
                                .HasColumnType("NUMBER(10)");

                            b1.Property<double>("Latitude")
                                .HasColumnType("BINARY_DOUBLE")
                                .HasColumnName("Latitude");

                            b1.Property<double>("Longitude")
                                .HasColumnType("BINARY_DOUBLE")
                                .HasColumnName("Longitude");

                            b1.HasKey("OcorrenciaId");

                            b1.ToTable("OD_OCORRENCIAS");

                            b1.WithOwner()
                                .HasForeignKey("OcorrenciaId");
                        });

                    b.Navigation("AreaRisco");

                    b.Navigation("Local")
                        .IsRequired();
                });

            modelBuilder.Entity("OlhoDagua.Cidadao.Domain.Entidades.AreaRisco", b =>
                {
                    b.Navigation("Abrigos");

                    b.Navigation("Alertas");

                    b.Navigation("Ocorrencias");
                });
#pragma warning restore 612, 618
        }
    }
}
