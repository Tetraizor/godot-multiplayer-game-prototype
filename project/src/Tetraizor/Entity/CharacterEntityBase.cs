namespace Tetraizor.Entity;

using Godot;
using Godot.Collections;
using Tetraizor.Autoloads;
using Tetraizor.Data;
using Tetraizor.Enums;

public abstract partial class CharacterEntityBase : CharacterBody2D, IEntity
{
    [Export] private float _movementSpeed = 128;

    public int EntityId { get; private set; }

    private Vector2 _targetPosition;
    private Vector2 _lastTargetPosition;
    private Vector2 _movementDirection;

    public EntityWrapper Wrapper { get; private set; }

    public override void _Ready()
    {
        base._Ready();
        _targetPosition = Position;
        _lastTargetPosition = Position;
        Name = EntityId + "";
    }

    public override void _Process(double dDelta)
    {
        float delta = (float)dDelta;

        if (_movementDirection != Vector2.Zero)
        {
            UpdateTargetPositionByDirection(_movementDirection);
        }
    }

    public override void _PhysicsProcess(double dDelta)
    {
        float delta = (float)dDelta;

        if ((_targetPosition - Position).Length() > 1)
        {
            Velocity = (_targetPosition - Position).Normalized() * _movementSpeed;
            MoveAndSlide();
        }
        else
        {
            Velocity = Vector2.Zero;
        }
    }

    public void UpdateTargetPosition(Vector2 newTargetPosition)
    {
        _lastTargetPosition = _targetPosition;
        _targetPosition = newTargetPosition;

        // if ((_lastTargetPosition - Position).Length() > 8) Position = _lastTargetPosition;
    }

    public void UpdateTargetPositionByDirection(Vector2 direction)
    {
        UpdateTargetPosition(Position + direction * _movementSpeed / NetworkManager.TICK_RATE);
    }

    public void SetMovementDirection(Vector2 direction)
    {
        _movementDirection = direction;
    }

    #region IEntity Implementation
    public abstract Dictionary Serialize();

    public virtual void Deserialize(Dictionary rawData)
    {
        SetEntityId((int)rawData["entity_id"]);

        if (rawData.ContainsKey("position"))
        {
            SetPosition((Vector2)rawData["position"]);
        }
    }

    public int GetEntityId() => EntityId;
    public void SetEntityId(int id) => EntityId = id;
    public Node2D GetNodeSelf() => this;
    #endregion
}