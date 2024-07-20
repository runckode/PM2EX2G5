using PM2EX2G5.Http;
using PM2EX2G5.Http.Requests;
using PM2EX2G5.Models;
using PM2EX2G5.Services;
using System.Diagnostics;
using System.Text.Json;

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
        PageEdit page = new PageEdit(sitio);
        Navigation.PushAsync(page);
    }

    private async void EliminarSitio(SitiosResponse sitio)
    {
        bool resultado = await DisplayAlert("Eliminar", "¿Desea eliminar este sitio?", "Sí", "No");
        if (resultado)
        {
            PeticionEliminar(sitio.Id);
            //PeticionEliminar(1);
            await ActualizarLista();
        }
    }

    private async void VerUbicacion(SitiosResponse sitio)
    {
        List<SitiosResponse> allSitios = await GetAllSitiosAsync();

        if (allSitios == null || allSitios.Count == 0)
        {
            allSitios = new List<SitiosResponse> { sitio };
        }

        if (!allSitios.Contains(sitio))
        {
            allSitios.Add(sitio);
        }

        PageLocation page = new PageLocation(sitio, allSitios);
        await Navigation.PushAsync(page);
    }

    //Radio de 100 metros
    private async Task<List<SitiosResponse>> GetAllSitiosAsync()
    {
        try
        {
            using HttpClient client = new HttpClient();
            HttpServiceConf config = new HttpServiceConf();
            string apiUrl = config.ConfigOnlineEndpoint("https://pm2-examen2-grupo5-api.onrender.com/api/sitios");

            HttpResponseMessage response = await client.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            List<SitiosResponse> sitios = JsonSerializer.Deserialize<List<SitiosResponse>>(responseBody);
            return sitios ?? new List<SitiosResponse>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error al obtener los sitios: " + ex.Message);
            return new List<SitiosResponse>();
        }
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

    public async Task PeticionEliminar(int id)
    {
        HttpClient _client = new HttpClient();

        HttpResponseMessage response;
        string url = $" https://pm2-examen2-grupo5-api.onrender.com/api/sitios/{id}";

        try
        {
            // Realizar la solicitud DELETE
            response = await _client.DeleteAsync(url);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Éxito", "Ítem eliminado exitosamente", "Aceptar");
            }
            else
            {
                string errorMessage = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Error", $"Hubo un error al eliminar el ítem: {errorMessage}", "Aceptar");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Se produjo una excepción: {ex.Message}", "Aceptar");
        }
    }

    private async Task ActualizarLista()
    {
        var list = await _service.GetRecordingsAsync();
        SitiosList.ItemsSource = list;
    }
}