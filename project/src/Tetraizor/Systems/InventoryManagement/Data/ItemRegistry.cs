namespace Tetraizor.Systems.InventoryManagement.Data;

using System.Collections.Generic;

public static class ItemRegistry
{
    // TODO: Temporary item list. Will be replaced to a system that reads
    // TODO: json files to load items. 
    public static List<Item> Items { get; private set; } = new() {
        new Item(0, "Air", "...", 64, null),
        new Item(1, "Log", "A log from a tree.", 64, "res://art/items/item_01.png"),
        new Item(2, "Plank", "A wooden plank.", 64, "res://art/items/item_02.png"),
        new Item(3, "Stick", "A wooden stick.", 64, "res://art/items/item_03.png"),
        new Item(4, "Stone", "A stone.", 64, "res://art/items/item_04.png"),
        new Item(5, "Iron Ore", "A piece of iron ore.", 64, "res://art/items/item_01.png"),
        new Item(6, "Iron Ingot", "A piece of iron ingot.", 64, "res://art/items/item_01.png"),
    };

    public static Item GetItem(int itemId)
    {
        return Items.Find(item => item.ItemId == itemId);
    }

    public static void RegisterItem(Item item)
    {
        Items.Add(item);
    }
}