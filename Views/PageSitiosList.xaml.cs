using PM2EX2G5.Http.Requests;
using PM2EX2G5.Models;
using PM2EX2G5.Services;

namespace PM2EX2G5.Views;

public partial class PageSitiosList : ContentPage
{
    private RecordedAudioHttpService _service;
    public PageSitiosList()
    {
        InitializeComponent();
        _service = new RecordedAudioHttpService();
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        var list = await _service.GetRecordingsAsync();
        SitiosList.ItemsSource = list;
        //List<Empleado> emplelist = new List<Empleado>();
        //emplelist = await Controllers.EmpleadosController.GetEmpleados();
        //listemple.ItemsSource = emplelist;
    }

    private async void OnItemSelected(object sender, EventArgs e)
    {
        var frame = sender as Frame;
        if (frame == null)
            return;

        var sitio = frame.BindingContext as SitiosResponse;

        string action = await DisplayActionSheet("Opciones", "Cancelar", null, "Editar", "Eliminar", "Ver Ubicación", "Audio");

        switch (action)
        {
            case "Editar":
                EditarSitio(sitio);
                break;
            case "Eliminar":
                EliminarSitio(sitio);
                break;
            case "Ver Ubicación":
                VerUbicacion(sitio);
                break;

            case "Audio":
                VerUbicacion(sitio);
                break;
        }
    }

    private void EditarSitio(SitiosResponse sitio)
    {
        PageSitios page = new PageSitios(/* sitio.Id */);
        Navigation.PushAsync(page);
    }

    private void EliminarSitio(SitiosResponse sitio)
    {
        PageSitios page = new PageSitios(/* sitio.Id, true */);
        Navigation.PushAsync(page);
    }

    private void VerUbicacion(SitiosResponse sitio)
    {
        PageLocation page = new PageLocation(sitio);
        Navigation.PushAsync(page);
    }
    private void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        PageSitios page = new PageSitios();
        Navigation.PushAsync(page);
    }

    private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        string searchText = e.NewTextValue.ToLower();
        List<SitiosResponse> list = await _service.GetRecordingsAsync();
        SitiosList.ItemsSource = list.Where(x => x.Descripcion.ToLower().Contains(searchText)).ToList();
    }

    private void SitiosList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {

    }
}