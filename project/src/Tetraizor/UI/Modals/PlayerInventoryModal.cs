namespace Tetraizor.UI.Modals;

using Godot;
using Tetraizor.Autoloads;
using Tetraizor.Systems.InventoryManagement;
using Tetraizor.Systems.InventoryManagement.Data;

public partial class PlayerInventoryModal : MoveableModalBase
{
    private Inventory _inventory;

    [Export] private InventorySlot[] _inventorySlots;

    public override void _Ready()
    {
        base._Ready();

        _inventory = new Inventory(_inventorySlots);

        _inventory.InsertItem(new ToolInstance(1), 1, 4);
        _inventory.InsertItem(new ToolInstance(2), 1, 6);
        _inventory.InsertItem(new ItemInstance(3), 32, 15);
        _inventory.InsertItem(new ItemInstance(4), 64, 18);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (Input.IsActionJustPressed("ui_toggle_inventory"))
        {
            ModalManager.ToggleModal(this);
        }
    }
}