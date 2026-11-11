using OlhoDagua.Cidadao.Application.Dtos;
using OlhoDagua.Cidadao.Domain.Entidades;

namespace OlhoDagua.Cidadao.Application.Mapeamento;

public static class Mapeadores
{
    public static AreaResponse ParaResponse(this AreaRisco a) =>
        new(a.Id, a.Nome, a.Bairro, a.Centro.Latitude, a.Centro.Longitude, a.RaioMetros, a.NivelRiscoHistorico);

    public static AlertaResponse ParaResponse(this Alerta alerta, AreaRisco area)
    {
        var baseResponse = new AlertaResponse(
            alerta.Id, alerta.Tipo, alerta.AreaRiscoId, area.Nome, alerta.Severidade,
            alerta.GerarMensagem(area), alerta.CalcularPrioridade(), alerta.Ativo,
            alerta.CriadoEm, alerta.EncerradoEm);

        return alerta switch
        {
            AlertaSensor s => baseResponse with { SensorCodigo = s.SensorCodigo, NivelCm = s.NivelCm, Faixa = s.Faixa },
            AlertaComunitario c => baseResponse with { QuantidadeRelatos = c.QuantidadeRelatos },
            AlertaPrevisao p => baseResponse with
            {
                ProbabilidadeChuvaForte = p.ProbabilidadeChuvaForte, JanelaHoras = p.JanelaHoras
            },
            _ => baseResponse
        };
    }

    public static OcorrenciaResponse ParaResponse(this Ocorrencia o)
    {
        var localPublico = o.Local.Aproximada();
        return new(o.Id, o.AreaRiscoId, o.Tipo, o.Descricao, localPublico.Latitude, localPublico.Longitude,
                   o.Status, o.Confirmacoes, o.CriadaEm, o.IdLocalApp);
    }

    public static AbrigoResponse ParaResponse(this Abrigo a) =>
        new(a.Id, a.AreaRiscoId, a.Nome, a.Endereco, a.Local.Latitude, a.Local.Longitude,
            a.Capacidade, a.Ocupacao, a.VagasLivres, a.Acessivel, a.AceitaAnimais);

    public static AbrigoProximoResponse ParaProximoResponse(this Abrigo a, double distanciaMetros) =>
        new(a.Id, a.AreaRiscoId, a.Nome, a.Endereco, a.Local.Latitude, a.Local.Longitude,
            a.Capacidade, a.Ocupacao, a.VagasLivres, a.Acessivel, a.AceitaAnimais, (int)Math.Round(distanciaMetros));

    public static InscricaoResponse ParaResponse(this Inscricao i) =>
        new(i.Id, i.DispositivoId, i.AreaRiscoId, i.PrefereFonteGrande, i.PrefereAltoContraste, i.CriadaEm, i.AtualizadaEm);
}
