namespace Tetraizor.Entity;

using Godot;
using Godot.Collections;
using Tetraizor.Data;

public interface IEntity : INetworkSerializable
{
    public Node2D GetNodeSelf();
    public int GetEntityId();
    public void SetEntityId(int id);
    public string GetPackedType() => GetNodeSelf().GetType().Name;
}