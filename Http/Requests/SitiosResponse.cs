namespace PM2EX2G5.Http.Requests;

public class SitiosResponse
{
    public int Id { get; set; }
    public double Latitud { get; set; } 
    public double Longitud { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public byte[]? FirmaDigital { get; set; }
    public byte[]? AudioFile { get; set; }
    public String? FirmaDigitalStr { get; set; }
    public String? AudioFileStr { get; set; }
}