using OlhoDagua.Cidadao.Domain.Entidades;
using OlhoDagua.Cidadao.Domain.Enums;
using OlhoDagua.Cidadao.Domain.Excecoes;
using OlhoDagua.Cidadao.Domain.ValueObjects;

namespace OlhoDagua.Cidadao.Tests.Dominio;

public class AlertaTests
{
    private static readonly DateTimeOffset Agora = new(2026, 11, 15, 18, 42, 0, TimeSpan.FromHours(-3));
    private static readonly AreaRisco Pantanal =
        new("Várzea do Jardim Pantanal", "São Miguel Paulista", new Coordenada(-23.4985, -46.4430), 1200, 5);

    [Theory]
    [InlineData(FaixaNivel.Atencao, Severidade.Media)]
    [InlineData(FaixaNivel.Alerta, Severidade.Alta)]
    [InlineData(FaixaNivel.Emergencia, Severidade.Critica)]
    public void AlertaSensor_define_severidade_pela_faixa(FaixaNivel faixa, Severidade esperada)
    {
        var alerta = new AlertaSensor(1, "SNS-PANTANAL-01", 235, faixa, Agora);

        Assert.Equal(esperada, alerta.Severidade);
    }

    [Fact]
    public void AlertaSensor_com_faixa_normal_e_rejeitado()
    {
        Assert.Throws<DominioException>(() => new AlertaSensor(1, "SNS-PANTANAL-01", 50, FaixaNivel.Normal, Agora));
    }

    [Fact]
    public void AlertaSensor_gera_mensagem_com_nivel_em_metros_no_padrao_brasileiro()
    {
        var alerta = new AlertaSensor(1, "SNS-PANTANAL-01", 235, FaixaNivel.Alerta, Agora);

        var mensagem = alerta.GerarMensagem(Pantanal);

        Assert.Contains("ALERTA", mensagem);
        Assert.Contains("Várzea do Jardim Pantanal", mensagem);
        Assert.Contains("2,35 m", mensagem);
    }

    [Fact]
    public void Datas_sao_guardadas_em_utc()
    {
        var alerta = new AlertaSensor(1, "SNS-PANTANAL-01", 235, FaixaNivel.Alerta, Agora);

        Assert.Equal(TimeSpan.Zero, alerta.CriadoEm.Offset);
        Assert.Equal(Agora, alerta.CriadoEm); // mesmo instante, só muda o fuso
    }

    [Fact]
    public void Prioridade_e_polimorfica_e_respeita_a_severidade()
    {
        Alerta[] alertas =
        [
            new AlertaPrevisao(1, 0.9, 3, Agora),                          // Alta + 18
            new AlertaSensor(1, "SNS-PANTANAL-01", 310, FaixaNivel.Emergencia, Agora), // Crítica + 30
            new AlertaComunitario(1, 3, Agora)                             // Alta + 15
        ];

        var ordenados = alertas.OrderByDescending(a => a.CalcularPrioridade()).Select(a => a.Tipo).ToArray();

        Assert.Equal(["SENSOR", "PREVISAO", "COMUNITARIO"], ordenados);
    }

    [Fact]
    public void Encerrar_duas_vezes_gera_conflito()
    {
        var alerta = new AlertaComunitario(1, 3, Agora);
        alerta.Encerrar(Agora.AddMinutes(30));

        Assert.False(alerta.Ativo);
        Assert.Throws<ConflitoDominioException>(() => alerta.Encerrar(Agora.AddMinutes(40)));
    }

    [Fact]
    public void Nao_encerra_antes_da_criacao()
    {
        var alerta = new AlertaComunitario(1, 3, Agora);

        Assert.Throws<DominioException>(() => alerta.Encerrar(Agora.AddMinutes(-1)));
    }

    [Fact]
    public void AlertaComunitario_reforcado_aumenta_relatos_e_prioridade()
    {
        var alerta = new AlertaComunitario(1, 3, Agora);
        var antes = alerta.CalcularPrioridade();

        alerta.RegistrarNovoRelato();

        Assert.Equal(4, alerta.QuantidadeRelatos);
        Assert.True(alerta.CalcularPrioridade() > antes);
        Assert.Contains("4 moradores", alerta.GerarMensagem(Pantanal));
    }

    [Theory]
    [InlineData(0.85, Severidade.Alta)]
    [InlineData(0.6, Severidade.Media)]
    [InlineData(0.3, Severidade.Baixa)]
    public void AlertaPrevisao_define_severidade_pela_probabilidade(double probabilidade, Severidade esperada)
    {
        Assert.Equal(esperada, new AlertaPrevisao(1, probabilidade, 3, Agora).Severidade);
    }

    [Fact]
    public void AlertaPrevisao_rejeita_probabilidade_fora_de_0_a_1()
    {
        Assert.Throws<DominioException>(() => new AlertaPrevisao(1, 1.2, 3, Agora));
    }
}
