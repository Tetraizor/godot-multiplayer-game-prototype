namespace Tetraizor.Managers;

using System;
using System.Collections.Generic;
using Godot;
using Tetraizor.Autoloads;
using Tetraizor.Data.Networking.Packet;
using Tetraizor.Entity;
using Tetraizor.Enums;

public partial class EntityController : Node
{
    private Dictionary<int, IEntity> _entities = new();
    public Dictionary<int, IEntity> Entities => _entities;
    public IEntity GetEntityById(int entityId) => _entities[entityId];

    [Export] private Godot.Collections.Dictionary<string, PackedScene> _entityScenes = new();
    [Export] private Node2D _container;

    private int _latestId = 0;
    private int GenerateId()
    {
        _latestId++;
        return _latestId;
    }

    /// <summary>
    /// Assigns the given order to some server defined properties like its entity ID.
    /// Can only be called BY and AT the server.
    /// </summary>
    public void CreateSpawnRequest(string packedType, Godot.Collections.Dictionary spawnOrder)
    {
        int networkId = NetworkManager.LocalProfile.NetworkId;

        if (networkId != NetworkManager.SERVER_ID)
        {
            CM.Error($"Only server can create a spawn request. Network ID: {networkId} Ignoring...");
            return;
        }

        // TODO: Do spawn order validations here.

        spawnOrder["entity_id"] = GenerateId();
        spawnOrder["packed_type"] = packedType;

        Rpc(MethodName.SpawnEntityOnAll, spawnOrder);
    }

    public void CreateSpawnRequestBatch(Godot.Collections.Array<Godot.Collections.Dictionary> spawnOrderPairs)
    {
        int networkId = NetworkManager.LocalProfile.NetworkId;

        if (networkId != NetworkManager.SERVER_ID)
        {
            CM.Error($"Only server can create a spawn request. Network ID: {networkId} Ignoring...");
            return;
        }

        Godot.Collections.Array<Godot.Collections.Dictionary> spawnOrdersArray = new();

        foreach (var spawnOrderPair in spawnOrderPairs)
        {
            var packedType = (string)(spawnOrderPair)["packed_type"];
            var spawnOrder = (Godot.Collections.Dictionary)(spawnOrderPair)["spawn_order"];

            // TODO: Do spawn order validations here.

            spawnOrder["entity_id"] = GenerateId();
            spawnOrder["packed_type"] = packedType;

            spawnOrdersArray.Add(spawnOrder);
        }

        Rpc("SpawnEntityBatchOnAll", spawnOrdersArray);
    }

    /// <summary>
    /// Method called by server ONLY, but can be called on any peer.
    /// Creates an entity with a predefined id.
    /// Only exception is SpawnEntitiesBatch method.
    /// </summary>
    [Rpc(mode: MultiplayerApi.RpcMode.Authority, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable, CallLocal = true)]
    private void SpawnEntityOnAll(Godot.Collections.Dictionary spawnOrder)
    {
        SpawnEntity(spawnOrder);
    }

    [Rpc(mode: MultiplayerApi.RpcMode.Authority, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable, CallLocal = true)]
    private void SpawnEntityBatchOnAll(Godot.Collections.Array<Godot.Collections.Dictionary> spawnOrders)
    {
        foreach (var spawnOrder in spawnOrders)
        {
            SpawnEntityOnAll(spawnOrder);
        }
    }

    /// <summary>
    /// For spawning an entity on the current client. Entity ID must have been generated by the server,
    /// or when calling locally, it should be appended to spawnOrder!
    /// </summary>
    public void SpawnEntity(Godot.Collections.Dictionary spawnOrder)
    {
        var packedType = (string)spawnOrder["packed_type"];
        var entityId = (int)spawnOrder["entity_id"];

        if (_entities.ContainsKey(entityId))
        {
            CM.Error($"Entity with entity ID {entityId} already exists!");
            return;
        }

        if (!_entityScenes.ContainsKey(packedType))
        {
            CM.Error($"No PackedScene registered with key {packedType}!");
            return;
        }

        Node node = _entityScenes[packedType].Instantiate();
        IEntity entity = (IEntity)node;
        entity.Deserialize(spawnOrder);

        _container.AddChild(node, true);
        _entities.Add(entity.GetEntityId(), entity);
    }

    /// <summary>
    /// Can only be called by the server. Clients can't despawn objects on their own, instead can request the server to do so by
    /// calling some method, which consequently calls this method.
    /// </summary>
    public void CreateDespawnRequest(int entityId)
    {
        int networkId = NetworkManager.LocalProfile.NetworkId;

        if (networkId != NetworkManager.SERVER_ID)
        {
            CM.Error($"Only server can create a despawn request. Network ID: {networkId} Ignoring...");
            return;
        }

        // TODO: Do despawn order validations here.
        Rpc(nameof(DespawnEntityOnAll), entityId);
    }

    [Rpc(mode: MultiplayerApi.RpcMode.Authority, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable, CallLocal = true)]
    public void DespawnEntityOnAll(int entityId)
    {
        DespawnEntity(entityId);
    }

    /// <summary>
    /// Should be called by server via DespawnEntityOnAll method. Calling it locally might cause issues.
    /// </summary>
    /// <param name="entityId"></param>
    public void DespawnEntity(int entityId)
    {
        if (!_entities.ContainsKey(entityId))
        {
            CM.Error($"Entity with entity ID {entityId} does not exist!");
            return;
        }

        var entity = _entities[entityId];

        entity.GetNodeSelf().QueueFree();
        _entities.Remove(entityId);
    }

    public IEntity[] GetEntitiesByAuthority(int networkId)
    {
        List<IEntity> entities = new();

        foreach (var entity in _entities.Values)
        {
            if (entity.GetNodeSelf().GetMultiplayerAuthority() == networkId)
            {
                entities.Add(entity);
            }
        }

        return entities.ToArray();
    }
}

public class EntityPositionUpdatePacket : PacketBase
{
    public Vector2 Position { get; set; }
    public int EntityId { get; set; }

    public EntityPositionUpdatePacket(int entityId, Vector2 position, int[] receivers = null) : base(PacketType.EntityPositionUpdate, 1, receivers)
    {
        Position = position;
        EntityId = entityId;
    }

    public override void Deserialize(Godot.Collections.Dictionary rawData)
    {
        Position = (Vector2)rawData["position"];
        EntityId = (int)rawData["entity_id"];
    }

    public override Godot.Collections.Dictionary Serialize()
    {
        return new Godot.Collections.Dictionary
        {
            {"position", Position},
            {"entity_id", EntityId},
        };
    }

    public static EntityPositionUpdatePacket From(Godot.Collections.Dictionary rawData)
    {
        return new EntityPositionUpdatePacket((int)rawData["entity_id"], (Vector2)rawData["position"]);
    }
}