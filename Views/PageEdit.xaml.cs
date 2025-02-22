using Android.App;
using Android.Media;
using Plugin.Maui.Audio;
using PM2EX2G5.Http.Commands;
using PM2EX2G5.Models;
using PM2EX2G5.Services;
using PM2EX2G5.ViewModels;
using System.Diagnostics;
using System.Text.Json;
using System.Text;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Text.Json.Serialization;
using static Android.Media.MediaRecorder;
using static Android.Util.EventLogTags;
using static Microsoft.Maui.ApplicationModel.Permissions;
using PM2EX2G5.Http.Requests;

namespace PM2EX2G5.Views;

public partial class PageEdit : ContentPage
{

    IAudioManager audioManager;
    readonly IDispatcher dispatcher;
    IAudioRecorder audioRecorder;
    AsyncAudioPlayer _audioPlayer;
    IAudioSource audioSource;
    private readonly Stopwatch recordingStopwatch;
    bool isPlaying;
    private GeolocationViewModel _geolcationViewModelo;
    private CancellationTokenSource _cancelTokenSource;
    private RecordedAudioHttpService _service;
    int id;
    public PageEdit(SitiosResponse sitio)
    {
        _geolcationViewModelo = new GeolocationViewModel();
        recordingStopwatch = new Stopwatch();
        InitializeComponent();
        audioManager = Plugin.Maui.Audio.AudioManager.Current;
        _service = new RecordedAudioHttpService();

        BindingContext = sitio;
        id = sitio.Id;

    }

    private void btnBorrar_Clicked(object sender, EventArgs e)
    {
        drawingViewFirma.Clear();
    }

    private void drawingViewFirma_DrawingLineStarted(object sender,
       CommunityToolkit.Maui.Core.DrawingLineStartedEventArgs e)
    {
    }

    private void OnSliderDragCompleted(object sender, EventArgs e)
    {
        if (_audioPlayer != null && _audioPlayer.CanSeek)
        {
            double data = PlaybackSlider.Value;
            _audioPlayer.Seek(data);
        }
    }

    private bool isRecording = false;
    private async void Start(object sender, EventArgs e)
    {

        if (isRecording)
        {
            isRecording = false;
            Stop(sender, e);
            return;
        }
        bool hasMicrofone = await ComprobarPermisos<Microphone>();

        if (hasMicrofone)
        {
            isRecording = true;
            audioRecorder = audioManager.CreateRecorder();
            await audioRecorder.StartAsync();
            btnStart.ImageSource = "stop.png";
        }

    }

    private async void Stop(object sender, EventArgs e)
    {
        audioSource = await audioRecorder.StopAsync();
        recordingStopwatch.Stop();

        btnStart.ImageSource = "play.png";
        btnGuardar.IsEnabled = true;
    }

    public static async Task<bool> ComprobarPermisos<TPermission>()
        where TPermission : BasePermission, new()
    {
        PermissionStatus status = await CheckStatusAsync<TPermission>();

        if (status == PermissionStatus.Granted)
        {
            return true;
        }

        status = await RequestAsync<TPermission>();

        return status == PermissionStatus.Granted;
    }

    private async void Actualizar(object sender, EventArgs e)
    {
        String latitud = LatitudText.Text;
        String longitud = LongitudText.Text;
        String descripcion = txtDescription.Text;

        //await DisplayAlert("Datos", "Descripcion: " + descripcion + "\nLatitud: " + latitud + "\nLongitud: " + longitud, "Aceptar");



        //var todoItem = new Sitios
        //{
        //    Id = id,
        //    Latitud = double.Parse(latitud),
        //    Longitud = double.Parse(longitud),
        //    Descripcion = descripcion
        //};

        if (audioSource is null)
        {
            return;
        }
        string base64Firma = await base64ImageFirma.ConvertToBase64Async(drawingViewFirma);
        System.IO.Stream stream = ((FileAudioSource)audioSource).GetAudioStream();
        byte[] audioBytes;
        using (MemoryStream ms = new MemoryStream())
        {
            await stream.CopyToAsync(ms);
            audioBytes = ms.ToArray();
        }

        string appDataPath = FileSystem.Current.AppDataDirectory;
        string filename = $"{DateTime.Now:yyyyMMddHHmmss}.wav";
        string filePath = Path.Combine(appDataPath, filename);
        await File.WriteAllBytesAsync(filePath, audioBytes);

        //audioBytes = new byte[0];
        string audiobase64 = Convert.ToBase64String(audioBytes);
        audioSource = null;
        btnGuardar.IsEnabled = false;

        var mysqlRecord = new CreateSitioCmd()
        {
            id = id,
            descripcion = txtDescription.Text,
            audioFile = audiobase64,
            firmaDigital = base64Firma,
            //AudioFile = audioBytes,
            //FirmaDigital = new byte[0],
            longitud = double.Parse(latitud),
            latitud = double.Parse(longitud)
        };
        //await _service.AddRecordingAsync(mysqlRecord);

        //await DisplayAlert("Exito", "�Sitio guardado correctamente!", "Ok");
        //txtDescription.Text = "";

        //Llamar al m�todo para guardar los cambios
        await PeticionActulizar(mysqlRecord);
    }

    private async void btnActulizarUbicacion(object sender, EventArgs e)
    {

        //BindingContext = _geolcationViewModelo;
        //await Task.Delay(2000);
        LatitudText.Text = _geolcationViewModelo.Latitude.ToString();
        LongitudText.Text = _geolcationViewModelo.Longitude.ToString();

    }

    public async Task PeticionActulizar(CreateSitioCmd item)
    {
        HttpClient _client;
        JsonSerializerOptions _serializerOptions;
        _client = new HttpClient();
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        // Crear el contenido de la solicitud
        String json = JsonSerializer.Serialize<CreateSitioCmd>(item, _serializerOptions);
        StringContent content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        HttpResponseMessage response;
        response = await _client.PutAsync("https://pm2-examen2-grupo5-api.onrender.com/api/sitios", content);

        if (response.IsSuccessStatusCode)
        {
            await DisplayAlert("�xito", "�tem actualizado exitosamente", "Aceptar");
        }
        else
        {
            await DisplayAlert("Error", "Hubo un error al guardar el �tem: " + response.RequestMessage, "Aceptar");
        }




    }
}