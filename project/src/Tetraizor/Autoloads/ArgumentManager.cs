using System.Collections.Generic;
using Godot;
using Tetraizor.Data.Networking;
using Tetrazior.Data.Networking;

namespace Tetraizor.Autoloads;

public partial class ArgumentManager : AutoloadBase<ArgumentManager>
{
    public Dictionary<string, string> Args { get; private set; }

    public override void _Ready()
    {
        base._Ready();

        Args = new Dictionary<string, string>();

        foreach (var argument in OS.GetCmdlineArgs())
        {
            if (argument.Contains('='))
            {
                string[] keyValue = argument.Split("=");
                Args[keyValue[0].TrimPrefix("--")] = keyValue[1];
            }
            else
            {
                // Options without an argument will be present in the dictionary,
                // with the value set to an empty string.
                Args[argument.TrimPrefix("--")] = "";
            }
        }

        bool autostart = false;

        int port = 0;
        string ip = "";
        string username = "";
        bool isServer = false;

        foreach (var arg in Args)
        {
            switch (arg.Key)
            {
                case "autostart":
                    autostart = true;
                    break;
                case "server":
                    isServer = true;
                    break;
                case "port":
                    port = int.Parse(arg.Value);
                    break;
                case "ip":
                    ip = arg.Value;
                    break;
                case "username":
                    username = arg.Value;
                    break;
            }
        }

        if (autostart)
        {
            if (port <= 0 || ip == "" || username == "")
            {
                CM.Error("Invalid arguments for autostart.");
                return;
            }

            if (isServer)
            {
                NetworkManager.Instance.Host(
                    new ServerProperties
                    {
                        Ip = ip,
                        Port = port,
                        MaxClients = NetworkManager.MAX_CLIENTS
                    },
                    new UserData
                    {
                        Username = username
                    }
                );
            }
            else
            {
                NetworkManager.Instance.Join(new UserData
                {
                    Username = username
                }, ip, port);
            }
        }
    }
}