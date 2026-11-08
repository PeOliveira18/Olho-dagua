using OlhoDagua.Cidadao.Domain.Excecoes;

namespace OlhoDagua.Cidadao.Domain.ValueObjects;

/// <summary>
/// Value object de posição geográfica. É imutável e se valida na criação,
/// então nunca existe uma coordenada fora da faixa válida.
/// </summary>
public sealed record Coordenada
{
    private const double RaioTerraMetros = 6_371_000;

    public double Latitude { get; private init; }
    public double Longitude { get; private init; }

    // Usado só pelo EF Core para materializar o owned type.
    private Coordenada() { }

    public Coordenada(double latitude, double longitude)
    {
        if (latitude is < -90 or > 90)
            throw new DominioException("Latitude deve estar entre -90 e 90.");
        if (longitude is < -180 or > 180)
            throw new DominioException("Longitude deve estar entre -180 e 180.");

        Latitude = latitude;
        Longitude = longitude;
    }

    /// <summary>Distância em metros até outra coordenada (fórmula de Haversine).</summary>
    public double DistanciaMetros(Coordenada outra)
    {
        ArgumentNullException.ThrowIfNull(outra);

        var dLat = ParaRadianos(outra.Latitude - Latitude);
        var dLon = ParaRadianos(outra.Longitude - Longitude);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
              + Math.Cos(ParaRadianos(Latitude)) * Math.Cos(ParaRadianos(outra.Latitude))
              * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return RaioTerraMetros * c;
    }

    /// <summary>
    /// Versão arredondada (3 casas ≈ 100 m), usada em respostas públicas
    /// para não expor a localização exata de quem fez um relato.
    /// </summary>
    public Coordenada Aproximada(int casasDecimais = 3) =>
        new(Math.Round(Latitude, casasDecimais), Math.Round(Longitude, casasDecimais));

    private static double ParaRadianos(double graus) => graus * Math.PI / 180;
}
