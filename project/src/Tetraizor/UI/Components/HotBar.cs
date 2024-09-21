namespace Tetraizor.UI.Components;

using Godot;
using Tetraizor.Systems.InventoryManagement;
using Tetraizor.Systems.InventoryManagement.Data;

public partial class HotBar : Control
{
    [Export] private InventorySlot[] _hotBarSlots;

    [Export] private Texture2D _defaultSlotGraphic;
    [Export] private Texture2D _selectedSlotGraphic;

    private Inventory _hotBarInventory;

    private int _selectedSlotId = 0;

    public override void _Ready()
    {
        base._Ready();
        _hotBarInventory = new Inventory(_hotBarSlots);

        UpdateUI();
    }

    private void UpdateUI()
    {
        foreach (var slot in _hotBarSlots)
        {
            slot.Texture = _defaultSlotGraphic;
        }

        _hotBarSlots[_selectedSlotId].Texture = _selectedSlotGraphic;
    }

    public void NextSlot()
    {
        _selectedSlotId++;
        if (_selectedSlotId > 8) _selectedSlotId = 0;

        UpdateUI();
    }

    public void PreviousSlot()
    {
        _selectedSlotId--;
        if (_selectedSlotId < 0) _selectedSlotId = 8;

        UpdateUI();
    }

    public void SelectSlot(int slot)
    {
        if (slot >= 0 && slot <= 8)
        {
            _selectedSlotId = slot;
            UpdateUI();
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
