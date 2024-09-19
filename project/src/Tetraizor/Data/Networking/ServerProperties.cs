namespace Tetraizor.Data.Networking;

public class ServerProperties
{
    public string Ip { get; set; } = null;
    public int Port { get; set; } = -1;
    public int MaxClients { get; set; } = 0;
}