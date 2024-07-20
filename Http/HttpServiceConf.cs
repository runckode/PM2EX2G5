namespace PM2EX2G5.Http;

public class HttpServiceConf
{
    public String ConfigOnlineEndpoint(string host) => $"https://pm2-examen2-grupo5-api.onrender.com/api/sitios";
    
    public String ConfigLocalEndpoint(string ip, int port) => $"https://pm2-examen2-grupo5-api.onrender.com/api/sitios";
}