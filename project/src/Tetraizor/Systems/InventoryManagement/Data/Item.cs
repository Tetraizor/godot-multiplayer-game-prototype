namespace Tetraizor.Systems.InventoryManagement.Data;

using Godot;
using Godot.Collections;
using Tetraizor.Data;

public class Item
{
    public int ItemId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int StackSize { get; set; }
    public Texture2D Texture { get; internal set; }

    public Item(int itemId, string name, string description, int stackSize, string spritePath = null)
    {
        ItemId = itemId;
        Name = name;
        Description = description;
        StackSize = stackSize;
        Texture = spritePath != null ? ResourceLoader.Load<Texture2D>(spritePath) : null;
    }
}