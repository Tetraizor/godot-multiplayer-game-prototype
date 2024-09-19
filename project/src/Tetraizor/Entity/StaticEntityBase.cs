using Godot;
using Godot.Collections;

namespace Tetraizor.Entity;

public abstract partial class StaticEntityBase : StaticBody2D, IEntity
{
    public int Id { get; private set; }

    public abstract Dictionary Serialize();
    public void Deserialize(Dictionary rawData)
    {
        SetEntityId((int)rawData["network_id"]);
        SetPosition((Vector2)rawData["position"]);
    }

    public int GetEntityId() => Id;
    public void SetEntityId(int id) => Id = id;
    public Node2D GetNodeSelf() => this;
}