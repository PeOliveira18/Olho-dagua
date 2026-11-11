using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OlhoDagua.Cidadao.Infrastructure.Persistencia.Migrations
{
    public partial class Inicial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OD_AREAS_RISCO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                    Bairro = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                    Latitude = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    Longitude = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    RaioMetros = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NivelRiscoHistorico = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OD_AREAS_RISCO", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OD_ABRIGOS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    AreaRiscoId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Nome = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                    Endereco = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    Latitude = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    Longitude = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    Capacidade = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Ocupacao = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Acessivel = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    AceitaAnimais = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OD_ABRIGOS", x => x.Id);
                    table.CheckConstraint("CK_OD_ABRIGOS_OCUPACAO", "\"Ocupacao\" >= 0 AND \"Ocupacao\" <= \"Capacidade\"");
                    table.ForeignKey(
                        name: "FK_OD_ABRIGOS_OD_AREAS_RISCO_AreaRiscoId",
                        column: x => x.AreaRiscoId,
                        principalTable: "OD_AREAS_RISCO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OD_ALERTAS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    AreaRiscoId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Severidade = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    CriadoEm = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    EncerradoEm = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true),
                    TipoAlerta = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    QuantidadeRelatos = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    ProbabilidadeChuvaForte = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    JanelaHoras = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    SensorCodigo = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    NivelCm = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    Faixa = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OD_ALERTAS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OD_ALERTAS_OD_AREAS_RISCO_AreaRiscoId",
                        column: x => x.AreaRiscoId,
                        principalTable: "OD_AREAS_RISCO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OD_INSCRICOES",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DispositivoId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    AreaRiscoId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    PrefereFonteGrande = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    PrefereAltoContraste = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CriadaEm = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    AtualizadaEm = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OD_INSCRICOES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OD_INSCRICOES_OD_AREAS_RISCO_AreaRiscoId",
                        column: x => x.AreaRiscoId,
                        principalTable: "OD_AREAS_RISCO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OD_OCORRENCIAS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    AreaRiscoId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Tipo = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    Descricao = table.Column<string>(type: "NVARCHAR2(280)", maxLength: 280, nullable: true),
                    Latitude = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    Longitude = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    Status = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    Confirmacoes = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CriadaEm = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    ResolvidaEm = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true),
                    IdLocalApp = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OD_OCORRENCIAS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OD_OCORRENCIAS_OD_AREAS_RISCO_AreaRiscoId",
                        column: x => x.AreaRiscoId,
                        principalTable: "OD_AREAS_RISCO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OD_ABRIGOS_AreaRiscoId",
                table: "OD_ABRIGOS",
                column: "AreaRiscoId");

            migrationBuilder.CreateIndex(
                name: "IX_OD_ALERTAS_AreaRiscoId_EncerradoEm",
                table: "OD_ALERTAS",
                columns: new[] { "AreaRiscoId", "EncerradoEm" });

            migrationBuilder.CreateIndex(
                name: "IX_OD_INSCRICOES_AreaRiscoId",
                table: "OD_INSCRICOES",
                column: "AreaRiscoId");

            migrationBuilder.CreateIndex(
                name: "IX_OD_INSCRICOES_DispositivoId",
                table: "OD_INSCRICOES",
                column: "DispositivoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OD_OCORRENCIAS_AreaRiscoId_CriadaEm",
                table: "OD_OCORRENCIAS",
                columns: new[] { "AreaRiscoId", "CriadaEm" });

            migrationBuilder.CreateIndex(
                name: "IX_OD_OCORRENCIAS_IdLocalApp",
                table: "OD_OCORRENCIAS",
                column: "IdLocalApp",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OD_ABRIGOS");

            migrationBuilder.DropTable(
                name: "OD_ALERTAS");

            migrationBuilder.DropTable(
                name: "OD_INSCRICOES");

            migrationBuilder.DropTable(
                name: "OD_OCORRENCIAS");

            migrationBuilder.DropTable(
                name: "OD_AREAS_RISCO");
        }
    }
}
