using OlhoDagua.Cidadao.Domain.Entidades;
using OlhoDagua.Cidadao.Domain.Enums;
using OlhoDagua.Cidadao.Domain.Excecoes;
using OlhoDagua.Cidadao.Domain.ValueObjects;

namespace OlhoDagua.Cidadao.Tests.Dominio;

public class AbrigoTests
{
    private static Abrigo NovoAbrigo(int capacidade = 10) =>
        new(1, "Abrigo Escola Municipal A (simulado)", "Rua Exemplo, 100", new Coordenada(-23.4990, -46.4425),
            capacidade, acessivel: true, aceitaAnimais: true);

    [Fact]
    public void Entrada_ocupa_vagas()
    {
        var abrigo = NovoAbrigo(10);

        abrigo.RegistrarEntrada(4);

        Assert.Equal(4, abrigo.Ocupacao);
        Assert.Equal(6, abrigo.VagasLivres);
        Assert.True(abrigo.TemVaga());
    }

    [Fact]
    public void Entrada_sem_vaga_suficiente_gera_conflito_e_nao_altera_ocupacao()
    {
        var abrigo = NovoAbrigo(10);
        abrigo.RegistrarEntrada(8);

        var erro = Assert.Throws<AbrigoSemVagaException>(() => abrigo.RegistrarEntrada(3));

        Assert.IsAssignableFrom<ConflitoDominioException>(erro);
        Assert.Equal(8, abrigo.Ocupacao);
    }

    [Fact]
    public void Abrigo_lotado_nao_tem_vaga()
    {
        var abrigo = NovoAbrigo(2);
        abrigo.RegistrarEntrada(2);

        Assert.False(abrigo.TemVaga());
    }

    [Fact]
    public void Saida_maior_que_a_ocupacao_e_rejeitada()
    {
        var abrigo = NovoAbrigo(10);
        abrigo.RegistrarEntrada(1);

        Assert.Throws<ConflitoDominioException>(() => abrigo.RegistrarSaida(2));
    }

    [Fact]
    public void Capacidade_precisa_ser_positiva()
    {
        Assert.Throws<DominioException>(() => NovoAbrigo(0));
    }
}

public class OcorrenciaTests
{
    private static readonly DateTimeOffset Agora = new(2026, 11, 15, 18, 0, 0, TimeSpan.Zero);

    private static Ocorrencia NovaOcorrencia(string? descricao = "Rua tomada até o joelho", DateTimeOffset? criadaEm = null) =>
        new(1, TipoOcorrencia.RuaAlagada, descricao, new Coordenada(-23.4985, -46.4430), Guid.NewGuid(), criadaEm ?? Agora);

    [Fact]
    public void Nasce_aberta_e_sem_confirmacoes()
    {
        var ocorrencia = NovaOcorrencia();

        Assert.Equal(StatusOcorrencia.Aberta, ocorrencia.Status);
        Assert.Equal(0, ocorrencia.Confirmacoes);
    }

    [Fact]
    public void Segunda_confirmacao_muda_status_para_confirmada()
    {
        var ocorrencia = NovaOcorrencia();

        ocorrencia.Confirmar();
        Assert.Equal(StatusOcorrencia.Aberta, ocorrencia.Status);

        ocorrencia.Confirmar();
        Assert.Equal(StatusOcorrencia.Confirmada, ocorrencia.Status);
    }

    [Fact]
    public void Nao_confirma_ocorrencia_resolvida()
    {
        var ocorrencia = NovaOcorrencia();
        ocorrencia.Resolver(Agora.AddHours(2));

        Assert.Throws<ConflitoDominioException>(ocorrencia.Confirmar);
    }

    [Fact]
    public void Descricao_acima_do_limite_e_rejeitada()
    {
        var textoLongo = new string('a', Ocorrencia.TamanhoMaximoDescricao + 1);

        Assert.Throws<DominioException>(() => NovaOcorrencia(textoLongo));
    }

    [Fact]
    public void Descricao_em_branco_vira_nula()
    {
        Assert.Null(NovaOcorrencia("   ").Descricao);
    }

    [Fact]
    public void So_relatos_recentes_e_nao_resolvidos_contam_para_o_alerta_comunitario()
    {
        var janela = TimeSpan.FromMinutes(60);
        var recente = NovaOcorrencia(criadaEm: Agora.AddMinutes(-10));
        var antiga = NovaOcorrencia(criadaEm: Agora.AddMinutes(-90));
        var resolvida = NovaOcorrencia(criadaEm: Agora.AddMinutes(-5));
        resolvida.Resolver(Agora);

        Assert.True(recente.ContaParaAlertaComunitario(Agora, janela));
        Assert.False(antiga.ContaParaAlertaComunitario(Agora, janela));
        Assert.False(resolvida.ContaParaAlertaComunitario(Agora, janela));
    }
}

public class CoordenadaAreaTests
{
    [Fact]
    public void Distancia_de_um_grau_de_longitude_no_equador_e_cerca_de_111_km()
    {
        var distancia = new Coordenada(0, 0).DistanciaMetros(new Coordenada(0, 1));

        Assert.InRange(distancia, 111_000, 111_400);
    }

    [Fact]
    public void Coordenadas_iguais_sao_iguais_por_valor()
    {
        Assert.Equal(new Coordenada(-23.5, -46.6), new Coordenada(-23.5, -46.6));
    }

    [Theory]
    [InlineData(-91, 0)]
    [InlineData(0, 181)]
    public void Coordenada_invalida_e_rejeitada(double latitude, double longitude)
    {
        Assert.Throws<DominioException>(() => new Coordenada(latitude, longitude));
    }

    [Fact]
    public void Coordenada_aproximada_arredonda_para_tres_casas()
    {
        Assert.Equal(new Coordenada(-23.499, -46.443), new Coordenada(-23.49876, -46.44312).Aproximada());
    }

    [Fact]
    public void Area_contem_pontos_dentro_do_raio()
    {
        var area = new AreaRisco("Várzea do Jardim Pantanal", "São Miguel Paulista", new Coordenada(-23.4985, -46.4430), 1200, 5);

        Assert.True(area.Contem(new Coordenada(-23.4990, -46.4425)));  // ~75 m
        Assert.False(area.Contem(new Coordenada(-23.5870, -46.6100))); // Ipiranga, ~19 km
    }
}
