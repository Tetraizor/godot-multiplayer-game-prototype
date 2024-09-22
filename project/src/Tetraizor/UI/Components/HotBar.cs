namespace Tetraizor.UI.Components;

using System;
using Godot;
using Tetraizor.Autoloads;
using Tetraizor.Controllers;
using Tetraizor.Systems.InventoryManagement;
using Tetraizor.Systems.InventoryManagement.Data;

public partial class HotBar : Control
{
    [Export] private InventorySlot[] _hotBarSlots;

    [Export] private Texture2D _defaultSlotGraphic;
    [Export] private Texture2D _selectedSlotGraphic;

    private Inventory _hotBarInventory;
    private PlayerController _playerController;
    private InventorySlot _selectedSlot;

    private int _selectedSlotId = 0;
    public delegate void SelectedSlotChangedEventHandler(InventorySlot slot);
    public event SelectedSlotChangedEventHandler SelectedSlotChanged;

    public override void _Ready()
    {
        base._Ready();
        _hotBarInventory = new Inventory(_hotBarSlots);
        _hotBarInventory.SlotInteracted += InventorySlotInteracted;
        _playerController = NodeManager.FindNodeOfType<PlayerController>();

        SelectUI();
    }

    private void InventorySlotInteracted(InventorySlot slot, bool isMainInteraction)
    {
        if (slot == _selectedSlot)
        {
            SelectUI();
        }
    }

    private void SelectUI()
    {
        foreach (var slot in _hotBarSlots)
        {
            slot.Texture = _defaultSlotGraphic;
        }

        _hotBarSlots[_selectedSlotId].Texture = _selectedSlotGraphic;
        _selectedSlot = _hotBarSlots[_selectedSlotId];
        SelectedSlotChanged?.Invoke(_selectedSlot);
    }

    public void NextSlot()
    {
        _selectedSlotId++;
        if (_selectedSlotId > 8) _selectedSlotId = 0;

        SelectUI();
    }

    public void PreviousSlot()
    {
        _selectedSlotId--;
        if (_selectedSlotId < 0) _selectedSlotId = 8;

        SelectUI();
    }

    public void SelectSlot(int slot)
    {
        if (slot >= 0 && slot <= 8)
        {
            _selectedSlotId = slot;
            SelectUI();
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.ButtonIndex == MouseButton.WheelUp && mouseButton.Pressed && !mouseButton.IsEcho())
            {
                PreviousSlot();
            }

            if (mouseButton.ButtonIndex == MouseButton.WheelDown && mouseButton.Pressed && !mouseButton.IsEcho())
            {
                NextSlot();
            }
        }

        if (@event is InputEventKey key)
        {
            if (key.Pressed && !key.IsEcho())
            {
                switch (key.Keycode)
                {
                    case Key.Key1:
                        SelectSlot(0);
                        break;
                    case Key.Key2:
                        SelectSlot(1);
                        break;
                    case Key.Key3:
                        SelectSlot(2);
                        break;
                    case Key.Key4:
                        SelectSlot(3);
                        break;
                    case Key.Key5:
                        SelectSlot(4);
                        break;
                    case Key.Key6:
                        SelectSlot(5);
                        break;
                    case Key.Key7:
                        SelectSlot(6);
                        break;
                    case Key.Key8:
                        SelectSlot(7);
                        break;
                    case Key.Key9:
                        SelectSlot(8);
                        break;
                }
            }
        }
    }
}
