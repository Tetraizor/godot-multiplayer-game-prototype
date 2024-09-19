using System;
using Godot;
using Tetraizor.Controllers;
using Tetraizor.Data.Networking;
using Tetrazior.Data.Networking;

namespace Tetraizor.Autoloads;

public partial class NetworkManager : AutoloadBase<NetworkManager>
{
    private bool _isConnectionActive;
    public bool IsConnectionActive => _isConnectionActive;

    public const string DEFAULT_IP = "";
    public const int DEFAULT_PORT = 4538;
    public const int MAX_CLIENTS = 4095;

    public const int SERVER_ID = 1;

    public const float CONNECTION_TIMEOUT_COUNT = 5;
    public const float CONNECTION_TRY_PERIOD_SECONDS = 1;

    public const int TICK_RATE = 60;

    private int _retryCounter = 0;
    private bool _isConnecting;

    public static bool IsServer => LocalProfile.NetworkId == SERVER_ID;
    public static PlayerProfile LocalProfile => Instance._localProfile;
    private PlayerProfile _localProfile;

    [Signal] public delegate void PeerConnectedEventHandler(long id);
    [Signal] public delegate void PeerDisconnectedEventHandler(long id);

    public async void Join(UserData userData, string ip = null, int port = -1)
    {
        if (_isConnecting)
        {
            CM.Error("Already connecting to a server.");
            // return;
        }

        _isConnecting = true;

        await ToSignal(GetTree().CreateTimer(1), Timer.SignalName.Timeout);

        var peer = new ENetMultiplayerPeer();
        var error = peer.CreateClient(
            ip ?? DEFAULT_IP,
            port == -1 ? DEFAULT_PORT : port
        );

        if (error != Error.Ok)
        {
            CM.Error($"Error creating client: {error}");
            _isConnecting = false;
            return;
        }

        Multiplayer.MultiplayerPeer = peer;

        if (!Multiplayer.IsConnected(MultiplayerApi.SignalName.ConnectionFailed, Callable.From(OnConnectionFailed)))
            Multiplayer.ConnectionFailed += OnConnectionFailed;
        if (!Multiplayer.IsConnected(MultiplayerApi.SignalName.ConnectedToServer, Callable.From(OnConnectedToServer)))
            Multiplayer.ConnectedToServer += OnConnectedToServer;
        if (!Multiplayer.IsConnected(MultiplayerApi.SignalName.ServerDisconnected, Callable.From(OnServerDisconnected)))
            Multiplayer.ServerDisconnected += OnServerDisconnected;
        if (!Multiplayer.IsConnected(MultiplayerApi.SignalName.PeerDisconnected, Callable.From<long>(OnPeerDisconnected)))
            Multiplayer.PeerDisconnected += OnPeerDisconnected;
        if (!Multiplayer.IsConnected(MultiplayerApi.SignalName.PeerConnected, Callable.From<long>(OnPeerConnected)))
            Multiplayer.PeerConnected += OnPeerConnected;

        _localProfile = new PlayerProfile
        {
            NetworkId = -1, // Set a placeholder IP to be corrected after connection is established.
            Username = userData.Username
        };

        double counter = 0;
        bool cancelConnection = false;

        while (
            Multiplayer.MultiplayerPeer.GetConnectionStatus() == MultiplayerPeer.ConnectionStatus.Connecting &&
            !cancelConnection
        )
        {
            counter += GetProcessDeltaTime();
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

            if (counter >= CONNECTION_TRY_PERIOD_SECONDS)
            {
                _isConnecting = false;
                cancelConnection = true;

                if (_retryCounter++ >= CONNECTION_TIMEOUT_COUNT)
                {
                    CM.Error("Connection timed out while trying to connect to a server.");
                    Disconnect(false);

                    _retryCounter = 0;
                }
                else
                {
                    CM.Error("Connection timed out while trying to connect to a server. Retrying...");
                    Disconnect(false);
                    Join(userData, ip, port);
                }

                return;
            }
        }

        TransitionManager.StartTransition();

        await ToSignal(TransitionManager.Instance, TransitionManager.SignalName.StartFinished);

        SceneManager.UnloadScene(1);
        SceneManager.LoadScene(2);

        _isConnectionActive = true;
        _isConnecting = false;
    }

    /// <summary>
    /// Host a server. If UserData is not provided, server will began in headless mode.
    /// </summary>
    /// <param name="serverProperties"></param>
    /// <param name="userData">Can be null, in which case server starts without local a user.</param>
    public async void Host(ServerProperties serverProperties, UserData userData = null)
    {
        if (_isConnecting)
        {
            CM.Error("Already connecting to a server.");
            return;
        }

        _isConnecting = true;

        var peer = new ENetMultiplayerPeer();
        var error = peer.CreateServer(
            serverProperties.Port == -1 ? DEFAULT_PORT : serverProperties.Port,
            serverProperties.MaxClients <= 0 ? MAX_CLIENTS : serverProperties.MaxClients
        );

        if (error != Error.Ok)
        {
            CM.Error($"Error creating server: {error}");
            return;
        }

        Multiplayer.MultiplayerPeer = peer;

        if (!Multiplayer.IsConnected(MultiplayerApi.SignalName.PeerDisconnected, Callable.From<long>(OnPeerDisconnected)))
            Multiplayer.PeerDisconnected += OnPeerDisconnected;
        if (!Multiplayer.IsConnected(MultiplayerApi.SignalName.PeerConnected, Callable.From<long>(OnPeerConnected)))
            Multiplayer.PeerConnected += OnPeerConnected;

        if (userData != null)
        {
            _localProfile = new PlayerProfile
            {
                NetworkId = SERVER_ID,
                Username = userData.Username
            };
        }
        TransitionManager.StartTransition();
        await ToSignal(TransitionManager.Instance, TransitionManager.SignalName.StartFinished);

        double counter = 0;
        bool cancelConnection = false;

        while (
            Multiplayer.MultiplayerPeer.GetConnectionStatus() == MultiplayerPeer.ConnectionStatus.Connecting &&
            !cancelConnection
        )
        {
            counter += GetProcessDeltaTime();
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

            if (counter >= CONNECTION_TIMEOUT_COUNT)
            {
                cancelConnection = true;

                CM.Error("Connection timed out while creating a server.");
            }
        }

        _isConnectionActive = true;

        SceneManager.UnloadScene(1);
        SceneManager.LoadScene(2);

        _isConnecting = false;
    }

    public void Disconnect(bool backToMenu = true)
    {
        CM.Print("Disconnecting...");

        Multiplayer.PeerDisconnected -= OnPeerDisconnected;
        Multiplayer.PeerConnected -= OnPeerConnected;

        if (_localProfile != null && _localProfile.NetworkId != SERVER_ID)
        {
            Multiplayer.ConnectionFailed -= OnConnectionFailed;
            Multiplayer.ConnectedToServer -= OnConnectedToServer;
            Multiplayer.ServerDisconnected -= OnServerDisconnected;
        }

        Multiplayer.MultiplayerPeer.Close();
        Multiplayer.MultiplayerPeer = null;
        _localProfile = null;

        if (!backToMenu) return;

        var task = async () =>
        {
            TransitionManager.StartTransition();

            await ToSignal(TransitionManager.Instance, TransitionManager.SignalName.StartFinished);

            _isConnectionActive = false;

            SceneManager.UnloadScene(2);
            SceneManager.LoadScene(1);
        };

        task.Invoke();
    }

    #region Common Callbacks
    private void OnPeerConnected(long id)
    {
        EmitSignal(nameof(PeerConnected), id);
    }

    private void OnPeerDisconnected(long id)
    {
        EmitSignal(nameof(PeerDisconnected), id);
    }
    #endregion

    #region Client Callbacks
    private void OnConnectedToServer()
    {
        // Correct IP address after connection is established.
        _localProfile.NetworkId = Multiplayer.GetUniqueId();
    }

    private void OnConnectionFailed()
    {
        Disconnect();
    }

    private void OnServerDisconnected()
    {
    }
    #endregion
}
