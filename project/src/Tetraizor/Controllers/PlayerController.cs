namespace Tetraizor.Controllers;

using Godot;
using Godot.Collections;
using Tetraizor.Autoloads;
using Tetraizor.Data.Networking.Packet;
using Tetraizor.Entity;
using Tetraizor.Enums;
using Tetraizor.Managers;
using Tetraizor.Utils;

public partial class PlayerController : Node
{
    [Export] private PlayerEntity _targetCharacter;
    [Export] private Camera2D _camera;
    private EntityController _entityController;

    private ReactiveProperty<Vector2> _reactivePosition = new();
    private ReactiveProperty<Vector2> _reactiveLookDirection = new();

    private bool _isTryingToUseTool = false;
    private float _toolCooldown = .4f;
    private float _toolCooldownTimer = 0;

    public override void _Ready()
    {
        _entityController = NodeManager.FindNodeOfType<EntityController>();

        _reactivePosition.ValueChanged += (oldValue, newValue) =>
        {
            Error error = RpcId(NetworkManager.SERVER_ID, nameof(SendInput), newValue, _targetCharacter.EntityId);
        };

        _reactiveLookDirection.ValueChanged += (oldValue, newValue) =>
        {
            if (_targetCharacter == null) return;

            Error error = _targetCharacter.RpcId(NetworkManager.SERVER_ID, nameof(PlayerEntity.SetPlayerStateRequest), new Dictionary {
                {"look_direction", newValue}
            });
            if (error != Error.Ok) CM.Error($"Error: {error}");
        };
    }

    public override void _Process(double dDelta)
    {
        if (_targetCharacter == null) return;
        if (!ScreenManager.IsFocused) return;

        _reactivePosition.Value = new Vector2(
            Input.GetAxis("move_left", "move_right"),
            Input.GetAxis("move_up", "move_down")
        ).Normalized();

        Vector2 direction = _camera.GetGlobalMousePosition() - _targetCharacter.Position;
        _reactiveLookDirection.Value = direction;

        _camera.GlobalPosition = _targetCharacter.Position;

        if (_toolCooldownTimer > 0) _toolCooldownTimer -= (float)dDelta;
        if (_isTryingToUseTool)
        {
            if (_toolCooldownTimer <= 0)
            {
                _toolCooldownTimer = _toolCooldown;
                _targetCharacter.RpcId(NetworkManager.SERVER_ID, nameof(PlayerEntity.SetPlayerStateRequest), new Dictionary {
                    {"is_using_tool", true}
                });
            }
        }
    }

    public void SetTarget(PlayerEntity playerEntity)
    {
        _targetCharacter = playerEntity;
    }

    [Rpc(mode: MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable, CallLocal = true)]
    public void SendInput(Vector2 direction, int entityId)
    {
        if (!NetworkManager.IsServer) return;

        var entity = _entityController.GetEntityById(entityId).GetNodeSelf() as PlayerEntity;
        var senderId = Multiplayer.GetRemoteSenderId();

        if (entity.GetMultiplayerAuthority() == senderId)
        {
            entity.SetMovementDirection(direction);
        }
        else
        {
            CM.Error($"PlayerController.SendInput: Unauthorized sender. Sender ID: {senderId}");
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);

        if (_targetCharacter == null) return;
        if (!ScreenManager.IsFocused) return;

        switch (@event)
        {
            case InputEventMouseButton mouseButton:
                if (mouseButton.IsEcho()) return;

                if (mouseButton.ButtonIndex == MouseButton.Left)
                {
                    _isTryingToUseTool = mouseButton.Pressed;
                }
                break;
        }
    }
}

public class InputPacket : PacketBase
{
    public InputPacket(Vector2 direction, int targetNetworkId, int[] receivers = null) : base(PacketType.PlayerControllerInput, -1, receivers)
    {
        Direction = direction;
        TargetNetworkId = targetNetworkId;
    }

    public Vector2 Direction { get; set; }
    public int TargetNetworkId { get; set; }

    public override void Deserialize(Dictionary rawData)
    {
        Direction = (Vector2)rawData["direction"];
        TargetNetworkId = (int)rawData["target_network_id"];
    }

    public override Dictionary Serialize()
    {
        return new Dictionary {
            {"direction", Direction},
            {"target_network_id", TargetNetworkId}
        };
    }
}