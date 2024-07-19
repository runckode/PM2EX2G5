namespace PM2EX2G5.Http;

public class HttpServiceConf
{
    public String ConfigOnlineEndpoint(string host) => $"https://{host}/api/recordinz";
    
    public String ConfigLocalEndpoint(string ip, int port) => $"http://{ip}:{port}/api/recordinz";
}