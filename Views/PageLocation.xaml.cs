using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using PM2EX2G5.Http.Requests;
using PM2EX2G5.Models;

namespace PM2EX2G5.Views;

public partial class PageLocation : ContentPage
{
    SitiosResponse _sitioSelected;
    public PageLocation(SitiosResponse sitios)
    {
        InitializeComponent();
        _sitioSelected = sitios;
        var location = new Location(Convert.ToDouble(sitios.Latitud), Convert.ToDouble(sitios.Longitud));
        ubicacion.MoveToRegion(MapSpan.FromCenterAndRadius(location, Distance.FromMiles(10)));
        var pin = new Pin
        {
            Label = sitios.Descripcion,
            Address = "Sitio Visitado",
            Type = PinType.Place,
            Location = location
        };

        ubicacion.Pins.Add(pin);
    }
}