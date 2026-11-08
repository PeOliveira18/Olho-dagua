using System.ComponentModel.DataAnnotations;
using OlhoDagua.Cidadao.Domain.Enums;

namespace OlhoDagua.Cidadao.Application.Dtos;

// ---------- Áreas de risco ----------

public sealed record AreaResponse(
    int Id, string Nome, string Bairro, double Latitude, double Longitude, int RaioMetros, int NivelRiscoHistorico);

public sealed record AreaDetalheResponse(
    int Id, string Nome, string Bairro, double Latitude, double Longitude, int RaioMetros, int NivelRiscoHistorico,
    int AlertasAtivos, int OcorrenciasAbertas);

public sealed record RankingAreaResponse(
    int AreaRiscoId, string Nome, string Bairro, int NivelRiscoHistorico, int TotalOcorrencias);

// ---------- Alertas ----------

/// <summary>Contrato ②: enviado pelo Serviço de Telemetria quando o nível sobe de faixa.</summary>
public sealed record CriarAlertaSensorRequest(
    [Range(1, int.MaxValue)] int AreaRiscoId,
    [Required, StringLength(40, MinimumLength = 3)] string SensorCodigo,
    [Range(0, 10_000)] double NivelCm,
    [Required] FaixaNivel? Faixa,
    DateTimeOffset? DetectadoEm);

/// <summary>Enviado pelo modelo de ML com a chance de chuva forte.</summary>
public sealed record CriarAlertaPrevisaoRequest(
    [Range(1, int.MaxValue)] int AreaRiscoId,
    [Range(0.0, 1.0)] double ProbabilidadeChuvaForte,
    [Range(1, 24)] int JanelaHoras);

public sealed record AlertaCriadoResponse(int AlertaId, string Mensagem);

/// <summary>
/// Visão única de qualquer tipo de alerta. Os campos específicos de cada
/// subclasse vêm preenchidos só quando se aplicam (os outros ficam nulos).
/// </summary>
public sealed record AlertaResponse(
    int Id,
    string Tipo,
    int AreaRiscoId,
    string AreaNome,
    Severidade Severidade,
    string Mensagem,
    int Prioridade,
    bool Ativo,
    DateTimeOffset CriadoEm,
    DateTimeOffset? EncerradoEm,
    string? SensorCodigo = null,
    double? NivelCm = null,
    FaixaNivel? Faixa = null,
    int? QuantidadeRelatos = null,
    double? ProbabilidadeChuvaForte = null,
    int? JanelaHoras = null);

// ---------- Ocorrências (relatos dos moradores) ----------

public sealed record RegistrarOcorrenciaRequest(
    int? AreaRiscoId,
    [Required] TipoOcorrencia? Tipo,
    [MaxLength(280)] string? Descricao,
    [Required, Range(-90.0, 90.0)] double? Latitude,
    [Required, Range(-180.0, 180.0)] double? Longitude,
    [Required] Guid? IdLocalApp);

/// <summary>
/// Visão pública de um relato. A coordenada é aproximada (~110 m) e a
/// descrição já passou pelo sanitizador de dados pessoais.
/// </summary>
public sealed record OcorrenciaResponse(
    int Id,
    int AreaRiscoId,
    TipoOcorrencia Tipo,
    string? Descricao,
    double Latitude,
    double Longitude,
    StatusOcorrencia Status,
    int Confirmacoes,
    DateTimeOffset CriadaEm,
    Guid IdLocalApp);

// ---------- Abrigos ----------

public sealed record AbrigoResponse(
    int Id, int AreaRiscoId, string Nome, string Endereco, double Latitude, double Longitude,
    int Capacidade, int Ocupacao, int VagasLivres, bool Acessivel, bool AceitaAnimais);

public sealed record AbrigoProximoResponse(
    int Id, int AreaRiscoId, string Nome, string Endereco, double Latitude, double Longitude,
    int Capacidade, int Ocupacao, int VagasLivres, bool Acessivel, bool AceitaAnimais, int DistanciaMetros);

public sealed record MovimentoAbrigoRequest([Range(1, 1000)] int Pessoas);

// ---------- Inscrições ----------

public sealed record SalvarInscricaoRequest(
    [Required] Guid? DispositivoId,
    [Range(1, int.MaxValue)] int AreaRiscoId,
    bool PrefereFonteGrande,
    bool PrefereAltoContraste);

public sealed record InscricaoResponse(
    int Id, Guid DispositivoId, int AreaRiscoId, bool PrefereFonteGrande, bool PrefereAltoContraste,
    DateTimeOffset CriadaEm, DateTimeOffset? AtualizadaEm);

/// <summary>Resultado de uma operação "cria ou devolve existente" (idempotente).</summary>
public sealed record Resultado<T>(T Valor, bool Criado);
