using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace PM2EX2G5.ViewModels;


public class GeolocationViewModel : INotifyPropertyChanged
{
    private double _latitude;
    public double Latitude
    {
        get { return _latitude; }
        set
        {
            if (_latitude != value)
            {
                _latitude = value;
                OnPropertyChanged(nameof(Latitude));
            }
        }
    }

    private double _longitude;
    public double Longitude
    {
        get { return _longitude; }
        set
        {
            if (_longitude != value)
            {
                _longitude = value;
                OnPropertyChanged(nameof(Longitude));
            }
        }
    }

    public GeolocationViewModel()
    {
        Task.Run(async () =>
        {
            GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
            Location location = await Geolocation.GetLocationAsync(request);

            Latitude = location.Latitude;
            Longitude = location.Longitude;
        });
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}