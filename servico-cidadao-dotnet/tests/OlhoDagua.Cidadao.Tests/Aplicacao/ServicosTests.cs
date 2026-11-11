using Microsoft.EntityFrameworkCore;
using OlhoDagua.Cidadao.Application.Dtos;
using OlhoDagua.Cidadao.Application.Pii;
using OlhoDagua.Cidadao.Domain.Entidades;
using OlhoDagua.Cidadao.Domain.Enums;
using OlhoDagua.Cidadao.Domain.Excecoes;

namespace OlhoDagua.Cidadao.Tests.Aplicacao;

public class RegraAlertaComunitarioTests
{
    private static RegistrarOcorrenciaRequest Relato(string? descricao = null) =>
        new(AreaRiscoId: null, TipoOcorrencia.RuaAlagada, descricao, -23.4990, -46.4425, Guid.NewGuid());

    [Fact]
    public async Task Dois_relatos_ainda_nao_geram_alerta()
    {
        using var cenario = new CenarioDeTeste();

        await cenario.Ocorrencias.RegistrarAsync(Relato());
        await cenario.Ocorrencias.RegistrarAsync(Relato());

        Assert.Empty(await cenario.Db.Alertas.ToListAsync());
        Assert.Empty(cenario.Publicador.Publicados);
    }

    [Fact]
    public async Task Terceiro_relato_gera_e_publica_um_alerta_comunitario()
    {
        using var cenario = new CenarioDeTeste();

        for (var i = 0; i < 3; i++) await cenario.Ocorrencias.RegistrarAsync(Relato());

        var alerta = Assert.Single(await cenario.Db.Alertas.OfType<AlertaComunitario>().ToListAsync());
        Assert.Equal(3, alerta.QuantidadeRelatos);
        Assert.Same(alerta, Assert.Single(cenario.Publicador.Publicados));
    }

    [Fact]
    public async Task Quarto_relato_reforca_o_alerta_em_vez_de_duplicar()
    {
        using var cenario = new CenarioDeTeste();

        for (var i = 0; i < 4; i++) await cenario.Ocorrencias.RegistrarAsync(Relato());

        var alerta = Assert.Single(await cenario.Db.Alertas.OfType<AlertaComunitario>().ToListAsync());
        Assert.Equal(4, alerta.QuantidadeRelatos);
        Assert.Single(cenario.Publicador.Publicados);
    }

    [Fact]
    public async Task Relatos_fora_da_janela_de_60_minutos_nao_contam()
    {
        using var cenario = new CenarioDeTeste();

        await cenario.Ocorrencias.RegistrarAsync(Relato());
        await cenario.Ocorrencias.RegistrarAsync(Relato());
        cenario.Relogio.Advance(TimeSpan.FromMinutes(61));
        await cenario.Ocorrencias.RegistrarAsync(Relato());

        Assert.Empty(await cenario.Db.Alertas.ToListAsync());
    }
}

public class OcorrenciaServiceTests
{
    [Fact]
    public async Task Reenviar_o_mesmo_relato_nao_duplica()
    {
        using var cenario = new CenarioDeTeste();
        var relato = new RegistrarOcorrenciaRequest(1, TipoOcorrencia.RuaAlagada, "Água na porta", -23.4990, -46.4425, Guid.NewGuid());

        var primeiro = await cenario.Ocorrencias.RegistrarAsync(relato);
        var segundo = await cenario.Ocorrencias.RegistrarAsync(relato);

        Assert.True(primeiro.Criado);
        Assert.False(segundo.Criado);
        Assert.Equal(primeiro.Valor.Id, segundo.Valor.Id);
        Assert.Equal(1, await cenario.Db.Ocorrencias.CountAsync());
    }

    [Fact]
    public async Task Descricao_e_salva_sem_dados_pessoais_e_local_sai_aproximado()
    {
        using var cenario = new CenarioDeTeste();
        var relato = new RegistrarOcorrenciaRequest(
            1, TipoOcorrencia.ImovelAtingido, "Meu CPF é 123.456.789-09, liga (11) 98888-7777",
            -23.49876, -46.44251, Guid.NewGuid());

        var resultado = await cenario.Ocorrencias.RegistrarAsync(relato);

        Assert.DoesNotContain("123.456.789", resultado.Valor.Descricao);
        Assert.DoesNotContain("98888", resultado.Valor.Descricao);
        Assert.Equal(-23.499, resultado.Valor.Latitude);
        Assert.Equal(-46.443, resultado.Valor.Longitude);
    }

    [Fact]
    public async Task Relato_sem_area_e_fora_das_areas_monitoradas_e_rejeitado()
    {
        using var cenario = new CenarioDeTeste();
        var relato = new RegistrarOcorrenciaRequest(null, TipoOcorrencia.RuaAlagada, null, -22.9, -43.2, Guid.NewGuid());

        await Assert.ThrowsAsync<DominioException>(() => cenario.Ocorrencias.RegistrarAsync(relato));
    }
}

public class AlertaServiceTests
{
    [Fact]
    public async Task Nova_faixa_do_mesmo_sensor_encerra_o_alerta_anterior()
    {
        using var cenario = new CenarioDeTeste();
        var agora = cenario.Relogio.GetUtcNow();

        var atencao = await cenario.Alertas.CriarAlertaSensorAsync(
            new CriarAlertaSensorRequest(1, "SNS-PANTANAL-01", 160, FaixaNivel.Atencao, agora));
        var alerta = await cenario.Alertas.CriarAlertaSensorAsync(
            new CriarAlertaSensorRequest(1, "sns-pantanal-01", 235, FaixaNivel.Alerta, agora.AddMinutes(12)));

        var ativos = await cenario.Alertas.ListarAsync(1, somenteAtivos: true);

        var unico = Assert.Single(ativos);
        Assert.Equal(alerta.AlertaId, unico.Id);
        Assert.Equal(FaixaNivel.Alerta, unico.Faixa);
        Assert.NotEqual(atencao.AlertaId, unico.Id);
    }

    [Fact]
    public async Task Encerrar_e_idempotente()
    {
        using var cenario = new CenarioDeTeste();
        var criado = await cenario.Alertas.CriarAlertaPrevisaoAsync(new CriarAlertaPrevisaoRequest(1, 0.85, 3));

        await cenario.Alertas.EncerrarAsync(criado.AlertaId);
        await cenario.Alertas.EncerrarAsync(criado.AlertaId);

        Assert.Empty(await cenario.Alertas.ListarAsync(1, somenteAtivos: true));
    }

    [Fact]
    public async Task Alerta_para_area_inexistente_e_rejeitado()
    {
        using var cenario = new CenarioDeTeste();

        await Assert.ThrowsAsync<DominioException>(() =>
            cenario.Alertas.CriarAlertaSensorAsync(new CriarAlertaSensorRequest(99, "SNS-X-01", 200, FaixaNivel.Alerta, null)));
    }
}

public class SanitizadorPiiTests
{
    private readonly SanitizadorPii _sanitizador = new();

    [Theory]
    [InlineData("CPF 123.456.789-09", "CPF ***.***.***-09")]
    [InlineData("cpf 12345678909 aqui", "cpf ***.***.***-09 aqui")]
    [InlineData("liga (11) 98888-7777", "liga (11) *****-7777")]
    [InlineData("tel 11 3333-4444", "tel (11) *****-4444")]
    [InlineData("email maria.silva@exemplo.com.br", "email m***@exemplo.com.br")]
    public void Mascara_dados_pessoais(string entrada, string esperado)
    {
        Assert.Equal(esperado, _sanitizador.Sanitizar(entrada));
    }

    [Fact]
    public void Texto_sem_dados_pessoais_fica_igual()
    {
        const string texto = "Rua alagada até o joelho perto do número 120, desde as 18h.";

        Assert.Equal(texto, _sanitizador.Sanitizar(texto));
    }
}
