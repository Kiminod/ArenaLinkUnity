namespace ArenaLink.ArenaLink.Config
{
    public class ArenaLinkOptions
    {
        public string BaseUrl { get; set; }
        public string ApiBaseUrl => $"{BaseUrl}/api";
        public string SignalRHubUrl => $"{BaseUrl}/GameHub";
        public string UdpServerAddress { get; set; }
        public int UdpServerPort { get; set; }
    }
}