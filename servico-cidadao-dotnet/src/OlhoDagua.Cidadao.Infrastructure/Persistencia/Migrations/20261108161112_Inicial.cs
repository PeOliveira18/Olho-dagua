using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OlhoDagua.Cidadao.Infrastructure.Persistencia.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AreasRisco",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Bairro = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    RaioMetros = table.Column<int>(type: "integer", nullable: false),
                    NivelRiscoHistorico = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AreasRisco", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Abrigos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AreaRiscoId = table.Column<int>(type: "integer", nullable: false),
                    Nome = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Endereco = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    Capacidade = table.Column<int>(type: "integer", nullable: false),
                    Ocupacao = table.Column<int>(type: "integer", nullable: false),
                    Acessivel = table.Column<bool>(type: "boolean", nullable: false),
                    AceitaAnimais = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Abrigos", x => x.Id);
                    table.CheckConstraint("CK_Abrigos_Ocupacao", "\"Ocupacao\" >= 0 AND \"Ocupacao\" <= \"Capacidade\"");
                    table.ForeignKey(
                        name: "FK_Abrigos_AreasRisco_AreaRiscoId",
                        column: x => x.AreaRiscoId,
                        principalTable: "AreasRisco",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Alertas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AreaRiscoId = table.Column<int>(type: "integer", nullable: false),
                    Severidade = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CriadoEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EncerradoEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    TipoAlerta = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    QuantidadeRelatos = table.Column<int>(type: "integer", nullable: true),
                    ProbabilidadeChuvaForte = table.Column<double>(type: "double precision", nullable: true),
                    JanelaHoras = table.Column<int>(type: "integer", nullable: true),
                    SensorCodigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    NivelCm = table.Column<double>(type: "double precision", nullable: true),
                    Faixa = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alertas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alertas_AreasRisco_AreaRiscoId",
                        column: x => x.AreaRiscoId,
                        principalTable: "AreasRisco",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Inscricoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DispositivoId = table.Column<Guid>(type: "uuid", nullable: false),
                    AreaRiscoId = table.Column<int>(type: "integer", nullable: false),
                    PrefereFonteGrande = table.Column<bool>(type: "boolean", nullable: false),
                    PrefereAltoContraste = table.Column<bool>(type: "boolean", nullable: false),
                    CriadaEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AtualizadaEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inscricoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inscricoes_AreasRisco_AreaRiscoId",
                        column: x => x.AreaRiscoId,
                        principalTable: "AreasRisco",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ocorrencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AreaRiscoId = table.Column<int>(type: "integer", nullable: false),
                    Tipo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(280)", maxLength: 280, nullable: true),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Confirmacoes = table.Column<int>(type: "integer", nullable: false),
                    CriadaEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ResolvidaEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IdLocalApp = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ocorrencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ocorrencias_AreasRisco_AreaRiscoId",
                        column: x => x.AreaRiscoId,
                        principalTable: "AreasRisco",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Abrigos_AreaRiscoId",
                table: "Abrigos",
                column: "AreaRiscoId");

            migrationBuilder.CreateIndex(
                name: "IX_Alertas_AreaRiscoId_EncerradoEm",
                table: "Alertas",
                columns: new[] { "AreaRiscoId", "EncerradoEm" });

            migrationBuilder.CreateIndex(
                name: "IX_Inscricoes_AreaRiscoId",
                table: "Inscricoes",
                column: "AreaRiscoId");

            migrationBuilder.CreateIndex(
                name: "IX_Inscricoes_DispositivoId",
                table: "Inscricoes",
                column: "DispositivoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ocorrencias_AreaRiscoId_CriadaEm",
                table: "Ocorrencias",
                columns: new[] { "AreaRiscoId", "CriadaEm" });

            migrationBuilder.CreateIndex(
                name: "IX_Ocorrencias_IdLocalApp",
                table: "Ocorrencias",
                column: "IdLocalApp",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Abrigos");

            migrationBuilder.DropTable(
                name: "Alertas");

            migrationBuilder.DropTable(
                name: "Inscricoes");

            migrationBuilder.DropTable(
                name: "Ocorrencias");

            migrationBuilder.DropTable(
                name: "AreasRisco");
        }
    }
}
