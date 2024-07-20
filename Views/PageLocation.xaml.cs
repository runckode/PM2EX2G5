using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using PM2EX2G5.Http.Requests;
using PM2EX2G5.Models;

namespace PM2EX2G5.Views;

public partial class PageLocation : ContentPage
{
    SitiosResponse _sitioSelected;
    List<SitiosResponse> _allSitios;

    public PageLocation(SitiosResponse sitios, List<SitiosResponse> allSitios)
    {
        InitializeComponent();
        _sitioSelected = sitios;
        _allSitios = allSitios;

        // Mostrar el sitio seleccionado en el mapa
        var location = new Location(sitios.Latitud, sitios.Longitud);
        ubicacion.MoveToRegion(MapSpan.FromCenterAndRadius(location, Distance.FromMiles(0.1)));

        // Agregar el pin del sitio seleccionado
        var selectedPin = new Pin
        {
            Label = sitios.Descripcion,
            Address = "Sitio Visitado",
            Type = PinType.Place,
            Location = location
        };
        ubicacion.Pins.Add(selectedPin);

        // Filtrar y agregar pines dentro de 100 metros
        foreach (var sitio in _allSitios)
        {
            double distance = CalculateDistance(sitios.Latitud, sitios.Longitud, sitio.Latitud, sitio.Longitud);
            if (distance <= 100)
            {
                var pin = new Pin
                {
                    Label = sitio.Descripcion,
                    Address = "Sitio Visitado",
                    Type = PinType.Place,
                    Location = new Location(sitio.Latitud, sitio.Longitud)
                };
                ubicacion.Pins.Add(pin);
            }
        }
    }

    public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371e3;

        double lat1Rad = lat1 * (Math.PI / 180);
        double lat2Rad = lat2 * (Math.PI / 180);
        double deltaLatRad = (lat2 - lat1) * (Math.PI / 180);
        double deltaLonRad = (lon2 - lon1) * (Math.PI / 180);

        double a = Math.Sin(deltaLatRad / 2) * Math.Sin(deltaLatRad / 2) +
                   Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                   Math.Sin(deltaLonRad / 2) * Math.Sin(deltaLonRad / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return R * c;
    }
}