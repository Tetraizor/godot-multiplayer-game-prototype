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
            if (slot.ItemInstance != null && _bufferInventorySlot.ItemInstance != null && slot.ItemInstance.ItemId == _bufferInventorySlot.ItemInstance.ItemId)
            {
                int leftover = (slot.Amount + _bufferInventorySlot.Amount) - slot.ItemInstance.StackSize;
                if (leftover > 0)
                {
                    slot.SetItem(slot.ItemInstance, slot.ItemInstance.StackSize);
                    _bufferInventorySlot.SetItem(slot.ItemInstance, leftover);
                }
                else
                {
                    slot.SetItem(slot.ItemInstance, slot.Amount + _bufferInventorySlot.Amount);
                    _bufferInventorySlot.SetItem(null);
                }
            }
            else
            {
                var previousItem = _bufferInventorySlot.ItemInstance;
                var previousAmount = _bufferInventorySlot.Amount;

                _bufferInventorySlot.SetItem(slot.ItemInstance, slot.Amount);
                slot.SetItem(previousItem, previousAmount);
            }
        }
        else
        {
            if (_bufferInventorySlot.ItemInstance == null)
            {
                var takeAmount = (int)Mathf.Ceil(slot.Amount / 2.0f);
                _bufferInventorySlot.SetItem(slot.ItemInstance, takeAmount);
                slot.RemoveItem(takeAmount);
            }
            else
            {
                if (slot.ItemInstance == null && _bufferInventorySlot.ItemInstance != null)
                {
                    slot.SetItem(_bufferInventorySlot.ItemInstance, 1);
                    _bufferInventorySlot.RemoveItem(1);
                }
                else if (_bufferInventorySlot.ItemInstance.ItemId == slot.ItemInstance.ItemId)
                {
                    _bufferInventorySlot.RemoveItem(1);
                    slot.AddItem(1);
                }
            }
        }
    }
}