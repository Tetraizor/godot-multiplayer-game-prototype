namespace Tetraizor.Entity;

using System;
using Godot;
using Godot.Collections;
using Tetraizor.Autoloads;
using Tetraizor.Controllers;
using Tetraizor.Data.Networking;
using Tetraizor.Enums;
using Tetraizor.Managers;
using Tetraizor.Utils;

public partial class PlayerEntity : CharacterEntityBase
{
    #region References
    [Export] private Label _usernameLabel;
    [Export] private AnimationTree _animationTree;
    [Export] private Node2D _rendererRoot;
    [Export] private Node2D _toolOrigin;
    [Export] private Sprite2D _toolRenderer;
    #endregion

    #region Player Properties
    public PlayerProfile Profile { get; private set; }
    public int Author { get; private set; } = -1;
    public string Username { get; private set; }
    #endregion

    #region State
    private bool _isUsingTool = false;

    private ReactiveProperty<Vector2> _reactivePosition = new();
    private Vector2 _lookDirection;
    #endregion

    #region Godot Methods
    public override void _Ready()
    {
        base._Ready();

        // Setup network related stuff.
        Profile = NodeManager.FindNodeOfType<GameController>().GetProfile(Author);
        Username = Profile.Username;

        SetMultiplayerAuthority(Profile.NetworkId);

        // Setup player.
        _usernameLabel.Text = Username;

        // Setup controller object.
        if (Author == NetworkManager.LocalProfile.NetworkId)
        {
            NodeManager.FindNodeOfType<PlayerController>().SetTarget(this);
        }

        // Setup callbacks.
        PacketBus.Instance.PacketReceived += OnPacketReceived;
        _reactivePosition.ValueChanged += (oldValue, newValue) =>
        {
            if (!NetworkManager.IsServer) return;

            var packet = new EntityPositionUpdatePacket(EntityId, newValue);
            PacketBus.QueuePacket(packet, true);
        };
    }

    public override void _ExitTree()
    {
        base._ExitTree();

        PacketBus.Instance.PacketReceived -= OnPacketReceived;
    }

    public override void _Process(double dDelta)
    {
        base._Process(dDelta);

        _reactivePosition.Value = Position;

        _animationTree.Set("parameters/BlendTree/tool_space/blend_position", _isUsingTool);
        _animationTree.Set("parameters/BlendTree/walk_space/blend_position", Velocity.Length());
    }

    public override void _Draw()
    {
        base._Draw();

        if (_lookDirection != Vector2.Zero)
        {
            float angle = _lookDirection.Angle();
            DrawArc(Vector2.Zero, 32, angle - Mathf.Pi / 2, angle + Mathf.Pi / 2, 12, new Color(1, 0, 0), 2);
        }
    }
    #endregion

    public void SetLookDirection(Vector2 direction)
    {
        _lookDirection = direction;

        if (direction == Vector2.Zero) return;
        direction = direction.Normalized();

        _toolOrigin.Rotation = new Vector2(Mathf.Abs(direction.X), direction.X > 0 ? direction.Y : -direction.Y).Angle();

        _toolOrigin.Scale = new Vector2(direction.X < 0 ? -1 : 1, 1);
        _rendererRoot.Scale = new Vector2(direction.X < 0 ? -1 : 1, 1);

        QueueRedraw();
    }

    #region Networking
    [Rpc(mode: MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Unreliable, CallLocal = true)]
    public void SetPlayerState(Dictionary state)
    {
        if (Multiplayer.GetRemoteSenderId() != NetworkManager.SERVER_ID) return;

        if (state.ContainsKey("look_direction"))
        {
            SetLookDirection((Vector2)state["look_direction"]);
        }

        if (state.ContainsKey("is_using_tool"))
        {
            _animationTree.Set("parameters/BlendTree/HitOneShot/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
        }
    }

    [Rpc(mode: MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Unreliable, CallLocal = true)]
    public void SetPlayerStateRequest(Dictionary state)
    {
        if (!NetworkManager.IsServer) return;

        Rpc(nameof(SetPlayerState), state);
    }

    private void OnPacketReceived(int senderId, PacketType type, Dictionary rawData)
    {
        switch (type)
        {
            case PacketType.EntityPositionUpdate:
                // TODO: Validate packet.
                if (NetworkManager.IsServer) return;

                var packet = EntityPositionUpdatePacket.From(rawData);
                if (packet.EntityId == EntityId)
                {
                    UpdateTargetPosition(packet.Position);
                }

                break;
        }
    }
    #endregion

    #region IEntity
    public override Dictionary Serialize()
    {
        return new Dictionary {
            {"username", _usernameLabel.Text},
            {"author", Author},
            {"position", Position}
        };
    }

    public override void Deserialize(Dictionary rawData)
    {
        base.Deserialize(rawData);
        Author = (int)rawData["author"];
    }
    #endregion
}
