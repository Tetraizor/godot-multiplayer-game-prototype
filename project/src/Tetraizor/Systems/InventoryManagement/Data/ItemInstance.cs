namespace Tetraizor.Systems.InventoryManagement.Data;

using Godot;
using Godot.Collections;
using Tetraizor.Data;

public class ItemInstance : INetworkSerializable
{
    public Item Item { get; private set; }

    public int ItemId => Item.ItemId;
    public int StackSize => Item.StackSize;
    public string Name => Item.Name;
    public string Description => Item.Description;
    public Texture2D Texture => Item.Texture;

    public ItemInstance(int itemId)
    {
        Item = ItemRegistry.GetItem(itemId);
    }

    public virtual void Deserialize(Dictionary rawData)
    {
        int itemId = (int)rawData["item_id"];

        Item = ItemRegistry.GetItem(itemId);
    }

    public virtual Dictionary Serialize()
    {
        return new Dictionary
        {
            {"item_id", ItemId},
        };
    }
}