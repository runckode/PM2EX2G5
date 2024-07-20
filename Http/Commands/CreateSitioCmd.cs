namespace PM2EX2G5.Http.Commands;

public class CreateSitioCmd
{
    public int id { get; set; }
    public double latitud { get; set; } 
    public double longitud { get; set; }
    public string descripcion { get; set; } = string.Empty;
    public string firmaDigital { get; set; }
    public string audioFile { get; set; }
}