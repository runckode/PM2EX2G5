using System.Diagnostics;
using PM2EX2G5.ViewModels;
using Plugin.Maui.Audio;
using PM2EX2G5.Http.Commands;
using PM2EX2G5.Services;

using static Microsoft.Maui.ApplicationModel.Permissions;

namespace PM2EX2G5.Views;

public partial class PageSitios : ContentPage
{
    IAudioManager audioManager;
    readonly IDispatcher dispatcher;
    IAudioRecorder audioRecorder;
    AsyncAudioPlayer _audioPlayer;
    IAudioSource audioSource ;
    private readonly Stopwatch recordingStopwatch;
    bool isPlaying;
    private GeolocationViewModel _geolcationViewModel;
    private CancellationTokenSource _cancelTokenSource;
    private RecordedAudioHttpService _service;

    public PageSitios()
    {
        _geolcationViewModel = new GeolocationViewModel();
        BindingContext = _geolcationViewModel;
        recordingStopwatch =  new Stopwatch();
        InitializeComponent();
        audioManager = AudioManager.Current;
        _service = new RecordedAudioHttpService();
    
    }



    protected async override void OnAppearing()
    {
        var res = await CheckAndRequestLocationPermission();
        bool isDisabled = res == PermissionStatus.Denied;
        if (isDisabled)
        {
            var response = DisplayAlert("Permissions GPS", "Activate", "Ok", "Close");
        }
    }

    public double RecordingTime
    {
        get => recordingStopwatch.ElapsedMilliseconds / 1000;
    }

    public bool IsPlaying
    {
        get => isPlaying;
        set => isPlaying = value;
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
            Stop(sender,e);
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

    private async void Guardar(object sender, EventArgs e)
    {
        if (audioSource is null)
        {
            return;
        }
        string base64Firma = await base64ImageFirma.ConvertToBase64Async(drawingViewFirma);
        Stream stream = ((FileAudioSource)audioSource).GetAudioStream();
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
            descripcion = txtDescription.Text,
            audioFile = audiobase64,
            firmaDigital = base64Firma,
            //AudioFile = audioBytes,
            //FirmaDigital = new byte[0],
            latitud = _geolcationViewModel.Latitude,
            longitud = _geolcationViewModel.Longitude
        };
        await _service.AddRecordingAsync(mysqlRecord);

        await DisplayAlert("Exito", "¡Sitio guardado correctamente!", "Ok");
        txtDescription.Text = "";
    }

    private async void Play(object sender, EventArgs e)
    {
        if (audioSource != null)
        {
            Stream d = ((FileAudioSource)audioSource).GetAudioStream();
            _audioPlayer = this.audioManager.CreateAsyncPlayer(d);

            isPlaying = true;
            await _audioPlayer.PlayAsync(CancellationToken.None);
            isPlaying = false;
        }
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

    public async Task<PermissionStatus> CheckAndRequestLocationPermission()
    {
        PermissionStatus status = await CheckStatusAsync<LocationAlways>();
        bool isDisabled = status == PermissionStatus.Denied;
        if (isDisabled)
        {
            status = await RequestAsync<
                LocationAlways>();

            return status;
        }

        if (status == PermissionStatus.Granted)
            return status;

        if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
        {
            return status;
        }


        status = await RequestAsync<LocationWhenInUse>();

        return status;
    }

    private void drawingViewFirma_DrawingLineStarted(object sender,
        CommunityToolkit.Maui.Core.DrawingLineStartedEventArgs e)
    {
    }

    private void btnBorrar_Clicked(object sender, EventArgs e)
    {
        drawingViewFirma.Clear();
    }
}