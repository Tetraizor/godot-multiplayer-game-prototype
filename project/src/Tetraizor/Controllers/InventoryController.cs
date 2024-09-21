namespace Tetraizor.Controllers;

using System;
using Godot;
using Tetraizor.Autoloads;
using Tetraizor.Systems.InventoryManagement;
using Tetraizor.Systems.InventoryManagement.Data;

public partial class InventoryController : Control
{
    private Inventory _bufferInventory;
    [Export] private InventorySlot _bufferInventorySlot;

    public override void _Ready()
    {
        base._Ready();

        _bufferInventory = new Inventory(_bufferInventorySlot);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        _bufferInventorySlot.GlobalPosition = GetViewport().GetMousePosition() - _bufferInventorySlot.Size / 2;

    }

    public override void _GuiInput(InputEvent @event)
    {
        base._GuiInput(@event);

        switch (@event)
        {
            case InputEventMouseMotion mouseMotion:
                break;
        }
    }

    public void OnSlotInteracted(InventorySlot slot, bool isMainInteraction)
    {
        if (isMainInteraction)
        {
            if (slot.Item != null && _bufferInventorySlot.Item != null && slot.Item.ItemId == _bufferInventorySlot.Item.ItemId)
            {
                int leftover = (slot.Amount + _bufferInventorySlot.Amount) - slot.Item.StackSize;
                if (leftover > 0)
                {
                    slot.SetItem(slot.Item, slot.Item.StackSize);
                    _bufferInventorySlot.SetItem(slot.Item, leftover);
                }
                else
                {
                    slot.SetItem(slot.Item, slot.Amount + _bufferInventorySlot.Amount);
                    _bufferInventorySlot.SetItem(null);
                }
            }
            else
            {
                var previousItem = _bufferInventorySlot.Item;
                var previousAmount = _bufferInventorySlot.Amount;

                _bufferInventorySlot.SetItem(slot.Item, slot.Amount);
                slot.SetItem(previousItem, previousAmount);
            }
        }
        else
        {
            if (_bufferInventorySlot.Item == null)
            {
                var takeAmount = (int)Mathf.Ceil(slot.Amount / 2.0f);
                _bufferInventorySlot.SetItem(slot.Item, takeAmount);
                slot.RemoveItem(takeAmount);
            }
            else
            {
                if (slot.Item == null && _bufferInventorySlot.Item != null)
                {
                    slot.SetItem(_bufferInventorySlot.Item, 1);
                    _bufferInventorySlot.RemoveItem(1);
                }
                else if (_bufferInventorySlot.Item.ItemId == slot.Item.ItemId)
                {
                    _bufferInventorySlot.RemoveItem(1);
                    slot.AddItem(1);
                }
            }
        }
    }
}