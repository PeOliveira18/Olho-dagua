using OlhoDagua.Cidadao.Domain.Excecoes;

namespace OlhoDagua.Cidadao.Domain.ValueObjects;

public sealed record Coordenada
{
    private const double RaioTerraMetros = 6_371_000;

    public double Latitude { get; private init; }
    public double Longitude { get; private init; }

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

    public Coordenada Aproximada(int casasDecimais = 3) =>
        new(Math.Round(Latitude, casasDecimais), Math.Round(Longitude, casasDecimais));

    private static double ParaRadianos(double graus) => graus * Math.PI / 180;
}
