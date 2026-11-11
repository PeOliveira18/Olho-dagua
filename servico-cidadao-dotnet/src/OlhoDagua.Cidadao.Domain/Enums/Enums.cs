namespace OlhoDagua.Cidadao.Domain.Enums;

public enum FaixaNivel
{
    Normal = 0,
    Atencao = 1,
    Alerta = 2,
    Emergencia = 3
}

public enum Severidade
{
    Baixa = 1,
    Media = 2,
    Alta = 3,
    Critica = 4
}

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
