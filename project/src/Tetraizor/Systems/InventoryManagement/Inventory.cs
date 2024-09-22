namespace Tetraizor.Systems.InventoryManagement;

using System;
using Tetraizor.Autoloads;
using Tetraizor.Controllers;
using Tetraizor.Systems.InventoryManagement.Data;

public class Inventory
{
    public delegate void SlotInteractedEventHandler(InventorySlot slot, bool isMainInteraction);
    public event SlotInteractedEventHandler SlotInteracted;

    private InventorySlot[] _slots = null;
    private InventorySlot _highlightedSlot;

    public Inventory(InventorySlot slot)
    {
        _slots = new InventorySlot[1];
        _slots[0] = slot;
        slot.Setup(this);
    }

    public Inventory(InventorySlot[] slots)
    {
        _slots = new InventorySlot[slots.Length];
        for (int i = 0; i < slots.Length; i++)
        {
            _slots[i] = slots[i];
            _slots[i].Setup(this);
        }
    }

    public int HasSlotFor(Item item, int count = 1)
    {
        if (item == null) return -1;

        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i] == null)
            {
                return i;
            }
            else
            {
                if (_slots[i].ItemInstance.ItemId == item.ItemId && _slots[i].Amount + count <= item.StackSize)
                {
                    return i;
                }
            }
        }

        return -1;
    }

    public bool InsertItem(ItemInstance itemInstance, int amount = 1, int slotId = -1)
    {
        if (itemInstance == null) return false;

        if (_slots[slotId].ItemInstance == null)
        {
            _slots[slotId].SetItem(itemInstance, amount);
            return true;
        }

        if (itemInstance.ItemId == _slots[slotId].ItemInstance.ItemId && amount + _slots[slotId].Amount <= _slots[slotId].ItemInstance.StackSize)
        {
            _slots[slotId].AddItem(amount);
            return true;
        }

        return false;
    }

    public void SetHighlightedSlot(InventorySlot inventorySlot)
    {
        _highlightedSlot = inventorySlot;
    }

    public void OnSlotInteracted(InventorySlot slot, bool isMainInteraction)
    {
        NodeManager.FindNodeOfType<InventoryController>().OnSlotInteracted(slot, isMainInteraction);
        SlotInteracted?.Invoke(slot, isMainInteraction);
    }
}