using Godot;

namespace Tetraizor.Entity;

public partial class EntityWrapper : RefCounted
{
    public int Id;
    public Node Entity;

    public EntityWrapper(int id, Node entity)
    {
        Id = id;
        Entity = entity;
    }
}