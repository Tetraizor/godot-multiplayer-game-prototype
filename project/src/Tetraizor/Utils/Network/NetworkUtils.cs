using System;

namespace Tetraizor.Utils.Network;

public static class NetworkUtils
{
    public static bool IsValidIpAddress(string ipAddress)
    {
        return System.Net.IPAddress.TryParse(ipAddress, out _);
    }

    public static bool IsValidDomain(string domain)
    {
        return Uri.CheckHostName(domain) != UriHostNameType.Unknown;
    }

    public static bool IsValidPort(string port)
    {
        return int.TryParse(port, out var portNumber) && portNumber >= 0 && portNumber <= 65535;
    }
}