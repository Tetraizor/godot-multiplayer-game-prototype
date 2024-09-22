namespace Tetraizor.Systems.InventoryManagement.Data;

using System.Collections.Generic;
using Tetraizor.Enums;

public static class ItemRegistry
{
    // TODO: Temporary item list. Will be replaced to a system that reads
    // TODO: json files to load items. 
    public static List<Item> Items { get; private set; } = new() {
        new Item(0, "Air", "...", 64, null),
        new Tool(1, "Axe", "Tool.", "res://art/items/item_01.png", 108, 32, 90, ToolType.Axe),
        new Tool(2, "Pickaxe", "Tool.", "res://art/items/item_02.png", 64, 48, 135, ToolType.Pickaxe),
        new Item(3, "Stick", "A wooden stick.", 64, "res://art/items/item_03.png"),
        new Item(4, "Log", "Wooden log from a tree.", 64, "res://art/items/item_04.png"),
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