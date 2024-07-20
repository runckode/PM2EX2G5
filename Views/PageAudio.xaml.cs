using Plugin.Maui.Audio;
using PM2EX2G5.Http.Requests;

namespace PM2EX2G5.Views;

public partial class PageAudio : ContentPage
{
    AsyncAudioPlayer _audioPlayer;
    private string _base64audi;
    public PageAudio(SitiosResponse sitios)
	{
		InitializeComponent();
        _base64audi = sitios.AudioFileStr;
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        PlayAudio();
    }

    private async void PlayAudio()
    {
        byte[] audioBytes = Convert.FromBase64String(_base64audi);
        MemoryStream audioStream = new MemoryStream(audioBytes);
        _audioPlayer = AudioManager.Current.CreateAsyncPlayer(audioStream);
        await _audioPlayer.PlayAsync(CancellationToken.None);
    }
}