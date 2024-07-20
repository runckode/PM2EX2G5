using System.Diagnostics;
using System.Text;
using System.Text.Json;
using PM2EX2G5.Http;
using PM2EX2G5.Http.Commands;
using PM2EX2G5.Http.Requests;


namespace PM2EX2G5.Services;

public class RecordedAudioHttpService
{
    HttpClient _client;
    JsonSerializerOptions _serializerOptions;

    private readonly int port = 5004;
    private readonly string apiUri  = String.Empty;
    public RecordedAudioHttpService()
    {

        apiUri = new HttpServiceConf().ConfigOnlineEndpoint( "typeserver.onrender.com");
        _client = new HttpClient();
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            WriteIndented = true
        };
    }

    public List<SitiosResponse> Items { get; private set; }
    

    public async Task AddRecordingAsync(CreateSitioCmd item, bool isNewItem = false)
    {
        
        Uri uri = new Uri(string.Format(apiUri, string.Empty));

        try
        {
            string json = JsonSerializer.Serialize(item);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            response = await _client.PostAsync(uri, content);

            if (response.IsSuccessStatusCode)
                Debug.WriteLine(@"\tGuardado Exitoso!!!");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
        }
    }
    
    public async Task<List<SitiosResponse>> GetRecordingsAsync()
    {
        Items = new List<SitiosResponse>();

        Uri uri = new Uri(string.Format(apiUri, string.Empty));
        HttpResponseMessage response = await _client.GetAsync(uri);
        
        bool isOk = response.IsSuccessStatusCode;
        if (isOk)
        {
            string content = await response.Content.ReadAsStringAsync();

            Items = JsonSerializer.Deserialize<List<SitiosResponse>>(content);
        }

        return Items;
    }
}