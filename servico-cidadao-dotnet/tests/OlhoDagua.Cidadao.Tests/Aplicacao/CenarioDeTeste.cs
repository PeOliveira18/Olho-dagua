using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;
using OlhoDagua.Cidadao.Application.Abstracoes;
using OlhoDagua.Cidadao.Application.Pii;
using OlhoDagua.Cidadao.Application.Servicos;
using OlhoDagua.Cidadao.Domain.Entidades;
using OlhoDagua.Cidadao.Domain.ValueObjects;
using OlhoDagua.Cidadao.Infrastructure.Persistencia;
using OlhoDagua.Cidadao.Infrastructure.Repositorios;

namespace OlhoDagua.Cidadao.Tests.Aplicacao;

public sealed class CenarioDeTeste : IDisposable
{
    public FakeTimeProvider Relogio { get; } = new(new DateTimeOffset(2026, 11, 15, 21, 0, 0, TimeSpan.Zero));
    public CidadaoDbContext Db { get; }
    public PublicadorEspiao Publicador { get; } = new();

    public OcorrenciaService Ocorrencias { get; }
    public AlertaService Alertas { get; }

    public CenarioDeTeste()
    {
        Db = new CidadaoDbContext(new DbContextOptionsBuilder<CidadaoDbContext>()
            .UseInMemoryDatabase($"cidadao-{Guid.NewGuid()}")
            .Options);

        var area = new AreaRisco("Várzea do Jardim Pantanal", "São Miguel Paulista", new Coordenada(-23.4985, -46.4430), 1200, 5);
        Db.Areas.Add(area);
        Db.Entry(area).Property(a => a.Id).CurrentValue = 1;
        Db.SaveChanges();

        var areaRepo = new AreaRiscoRepositorio(Db);
        var alertaRepo = new AlertaRepositorio(Db);
        var ocorrenciaRepo = new OcorrenciaRepositorio(Db);

        var regra = new RegraAlertaComunitario(ocorrenciaRepo, alertaRepo, Relogio);
        Ocorrencias = new OcorrenciaService(ocorrenciaRepo, areaRepo, regra, Publicador, new SanitizadorPii(), Relogio);
        Alertas = new AlertaService(alertaRepo, areaRepo, Publicador, Relogio);
    }

    public void Dispose() => Db.Dispose();
}

public sealed class PublicadorEspiao : IPublicadorAlertas
{
    public List<Alerta> Publicados { get; } = [];

    public Task PublicarAsync(Alerta alerta, AreaRisco area, CancellationToken ct = default)
    {
        Publicados.Add(alerta);
        return Task.CompletedTask;
    }
}
