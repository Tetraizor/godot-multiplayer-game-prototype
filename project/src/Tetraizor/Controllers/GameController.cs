namespace Tetraizor.Controllers;

using Godot;
using Godot.Collections;
using Tetraizor.Autoloads;
using Tetraizor.Data.Networking;
using Tetraizor.Entity;
using Tetraizor.Managers;

public partial class GameController : Node
{
    // public System.Collections.Generic.Dictionary<int, PlayerWrapper> _players = new();
    public System.Collections.Generic.Dictionary<int, PlayerProfile> _players = new();
    public PlayerProfile GetProfile(int id) => _players[id];

    private EntityController _entityController;

    public override void _Ready()
    {
        // Means player joined the game, and ready to load the game & play.

        _entityController = NodeManager.FindNodeOfType<EntityController>();

        RpcId(1, "OnPlayerJoined", NetworkManager.LocalProfile.Serialize());
        GetWindow().Title = $"{NetworkManager.LocalProfile.Username} ({NetworkManager.LocalProfile.NetworkId})";

        NetworkManager.Instance.PeerConnected += OnPeerConnected;
        NetworkManager.Instance.PeerDisconnected += OnPeerDisconnected;
    }

    private void OnPeerDisconnected(long id)
    {
        if (NetworkManager.IsServer)
        {
            _players.Remove((int)id);

            foreach (IEntity entity in _entityController.GetEntitiesByAuthority((int)id))
            {
                _entityController.CreateDespawnRequest(entity.GetEntityId());
            }
        }
    }

    private void OnPeerConnected(long id) { }

    /// <summary>
    /// Called from a newly joined players client. Goes to server only.
    /// Server sends snapshot of the world to notifying client.
    /// </summary>
    [Rpc(mode: MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable, CallLocal = true)]
    private void OnPlayerJoined(Dictionary profileData)
    {
        if (!NetworkManager.IsServer) return;

        // Server receives the notification that new person joined the game.
        var newPlayerProfile = PlayerProfile.From(profileData);
        _players.Add(newPlayerProfile.NetworkId, newPlayerProfile);

        if (newPlayerProfile.NetworkId == NetworkManager.SERVER_ID)
        {
            // TODO: Retrieve world state from physical save data.
        }
        else
        {
            SendSnapshot(newPlayerProfile.NetworkId);
        }

        // TODO: Spawn new player.
        _entityController.CreateSpawnRequest(typeof(PlayerEntity).Name, new Dictionary {
            {"position", new Vector2(System.Random.Shared.Next(400, 600), System.Random.Shared.Next(400, 600))},
            {"author", newPlayerProfile.NetworkId}
        });
    }

    private void SendSnapshot(int receiverId)
    {
        var playerData = new Array();
        var worldData = new Dictionary();
        var entityData = new Array();

        // Serialize Player Data (Profiles only)
        foreach (var player in _players)
        {
            playerData.Add(player.Value.Serialize());
        }

        // Serialize World Data
        // TODO: Create a world to serialize first :(

        // Serialize Entity Data
        var entities = NodeManager.FindNodeOfType<EntityController>().Entities;

        foreach (var entity in entities)
        {
            var spawnOrder = entity.Value.Serialize();
            spawnOrder["entity_id"] = entity.Key;
            spawnOrder["packed_type"] = entity.Value.GetPackedType();

            entityData.Add(spawnOrder);
        }

        var snapshot = new Dictionary {
            {"player_data", playerData},
            {"world_data", worldData},
            {"entity_data", entityData},
        };

        RpcId(receiverId, "ReceiveSnapshot", snapshot);
    }

    /// <summary>
    /// Receives latest snapshot of the server, applies it to the current game scene.
    /// After initialization finishes, requests for it's own player to be spawned.
    /// </summary>
    [Rpc(mode: MultiplayerApi.RpcMode.Authority, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable, CallLocal = true)]
    private void ReceiveSnapshot(Dictionary snapshotDataRaw)
    {
        // Process snapshot.
        // Process player profiles
        Array playerProfileList = (Array)snapshotDataRaw["player_data"];
        foreach (Dictionary player in playerProfileList)
        {
            var playerProfile = PlayerProfile.From(player);
            _players.Add(playerProfile.NetworkId, playerProfile);
        }

        // Process world data

        // Spawn entities

        Array<Dictionary> entitiesList = (Array<Dictionary>)snapshotDataRaw["entity_data"];

        foreach (Dictionary entity in entitiesList)
        {
            entity["entity_id"] = (int)entity["entity_id"];
            _entityController.SpawnEntity(entity);
        }
    }
}