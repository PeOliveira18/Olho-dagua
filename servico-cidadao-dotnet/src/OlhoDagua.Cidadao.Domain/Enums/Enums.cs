namespace OlhoDagua.Cidadao.Domain.Enums;

/// <summary>
/// Faixa do nível do córrego, calculada pelo Serviço de Telemetria.
/// Na API trafega em MAIÚSCULAS: NORMAL, ATENCAO, ALERTA, EMERGENCIA.
/// </summary>
public enum FaixaNivel
{
    Normal = 0,
    Atencao = 1,
    Alerta = 2,
    Emergencia = 3
}

/// <summary>Gravidade de um alerta. O valor numérico é usado no cálculo de prioridade.</summary>
public enum Severidade
{
    Baixa = 1,
    Media = 2,
    Alta = 3,
    Critica = 4
}

/// <summary>O que o morador está vendo. Na API: RUA_ALAGADA, CORREGO_TRANSBORDANDO, ...</summary>
public enum TipoOcorrencia
{
    RuaAlagada,
    CorregoTransbordando,
    ImovelAtingido,
    BueiroEntupido
}

public enum StatusOcorrencia
{
    Aberta,
    Confirmada,
    Resolvida
}
