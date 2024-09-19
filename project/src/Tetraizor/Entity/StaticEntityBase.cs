using Godot;
using Godot.Collections;

namespace Tetraizor.Entity;

public abstract partial class StaticEntityBase : StaticBody2D, IEntity
{
    public int EntityId { get; private set; }

    public override void _Ready()
    {
        base._Ready();

        Name = EntityId + "";
    }

    public virtual void Deserialize(Dictionary rawData)
    {
        SetEntityId((int)rawData["entity_id"]);
        SetPosition((Vector2)rawData["position"]);
    }

    public virtual Dictionary Serialize()
    {
        return new Dictionary {
            {"position", Position},
        };
    }

    public int GetEntityId() => EntityId;
    public void SetEntityId(int id) => EntityId = id;
    public Node2D GetNodeSelf() => this;

    public void Interact()
    {
        throw new System.NotImplementedException();
    }
}