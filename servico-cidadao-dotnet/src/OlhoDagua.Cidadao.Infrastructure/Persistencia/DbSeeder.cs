using Microsoft.EntityFrameworkCore;
using OlhoDagua.Cidadao.Domain.Entidades;
using OlhoDagua.Cidadao.Domain.ValueObjects;

namespace OlhoDagua.Cidadao.Infrastructure.Persistencia;

/// <summary>
/// Dados de seed da seção B4 do PLANO.md. Todos simulados (nomes genéricos, coordenadas aproximadas).
/// Os IDs das áreas são fixos porque o Serviço de Telemetria referencia areaRiscoId 1, 2 e 3.
/// </summary>
public static class DbSeeder
{
    public static async Task SeedAsync(CidadaoDbContext db, CancellationToken ct = default)
    {
        if (await db.Areas.AnyAsync(ct)) return;

        var areas = new (int Id, AreaRisco Area)[]
        {
            (1, new AreaRisco("Várzea do Jardim Pantanal", "São Miguel Paulista", new Coordenada(-23.4985, -46.4430), 1200, 5)),
            (2, new AreaRisco("Jardim Romano", "Itaim Paulista", new Coordenada(-23.4890, -46.3960), 1000, 4)),
            (3, new AreaRisco("Córrego Ipiranga", "Ipiranga", new Coordenada(-23.5870, -46.6100), 800, 3)),
        };
        foreach (var (id, area) in areas)
        {
            db.Areas.Add(area);
            db.Entry(area).Property(a => a.Id).CurrentValue = id;
        }

        db.Abrigos.AddRange(
            new Abrigo(1, "Abrigo Escola Municipal A (simulado)", "Endereço ilustrativo, Jardim Pantanal",
                       new Coordenada(-23.4960, -46.4470), 120, acessivel: true, aceitaAnimais: true),
            new Abrigo(1, "Abrigo Centro Comunitário B (simulado)", "Endereço ilustrativo, Jardim Pantanal",
                       new Coordenada(-23.5010, -46.4390), 60, acessivel: false, aceitaAnimais: false),
            new Abrigo(2, "Abrigo Ginásio C (simulado)", "Endereço ilustrativo, Jardim Romano",
                       new Coordenada(-23.4870, -46.3990), 200, acessivel: true, aceitaAnimais: false),
            new Abrigo(3, "Abrigo Paróquia D (simulado)", "Endereço ilustrativo, Ipiranga",
                       new Coordenada(-23.5850, -46.6080), 80, acessivel: true, aceitaAnimais: false));

        await db.SaveChangesAsync(ct);

        // IDs inseridos manualmente não avançam a sequência do PostgreSQL: ajusta para o próximo insert não colidir.
        if (db.Database.IsRelational())
            await db.Database.ExecuteSqlRawAsync(
                "SELECT setval(pg_get_serial_sequence('\"AreasRisco\"', 'Id'), (SELECT MAX(\"Id\") FROM \"AreasRisco\"));", ct);
    }
}
