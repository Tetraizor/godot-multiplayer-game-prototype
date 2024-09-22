namespace Tetraizor.Systems.InventoryManagement.Data;

using Godot;

public partial class InventorySlot : TextureRect
{
    [Export] private Label _countLabel;
    [Export] private TextureRect _itemDisplay;

    private Inventory _connectedInventory;

    public ItemInstance ItemInstance { get; private set; }
    public int Amount { get; private set; }

    private bool _isHovering;

    public override void _Ready()
    {
        ClearItem();

        MouseEntered += OnHoverStart;
        MouseExited += OnHoverEnd;
    }

    public void Setup(Inventory inventory, ItemInstance itemInstance = null, int amount = 0)
    {
        _connectedInventory = inventory;
        ItemInstance = itemInstance;
        Amount = amount;
    }

    public override void _ExitTree()
    {
        base._ExitTree();

        MouseEntered -= OnHoverStart;
        MouseExited -= OnHoverEnd;
    }

    private void OnHoverStart()
    {
        SelfModulate = new Color(1, 1, 1, .5f);
        _connectedInventory.SetHighlightedSlot(this);
    }
    private void OnHoverEnd()
    {
        SelfModulate = new Color(1, 1, 1, 1);
    }

    public void SetItem(ItemInstance itemInstance, int amount = 1)
    {
        if (itemInstance == null)
        {
            ClearItem();
            return;
        }

        Amount = amount;
        ItemInstance = itemInstance;

        _itemDisplay.Texture = itemInstance.Texture;
        _countLabel.Text = Amount > 1 ? Amount.ToString() : "";
    }

    public void ClearItem()
    {
        Amount = 0;
        ItemInstance = null;

        _itemDisplay.Texture = null;
        _countLabel.Text = "";
    }

    public void AddItem(int amount)
    {
        if (amount < 0 || ItemInstance == null || (Amount + amount) > ItemInstance.StackSize) return;

        Amount += amount;

        _countLabel.Text = Amount > 1 ? Amount.ToString() : "";
    }

    public void RemoveItem(int amount)
    {
        if (amount < 0 || Amount < amount || ItemInstance == null) return;
        Amount -= amount;

        _countLabel.Text = Amount > 1 ? Amount.ToString() : "";

        if (Amount <= 0) ClearItem();
    }

    public override void _GuiInput(InputEvent @event)
    {
        base._GuiInput(@event);

        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.Pressed)
            {
                if (mouseButton.ButtonIndex == MouseButton.Left)
                {
                    _connectedInventory.OnSlotInteracted(this, true);
                }
                else if (mouseButton.ButtonIndex == MouseButton.Right)
                {
                    _connectedInventory.OnSlotInteracted(this, false);
                }
            }
        }
    }

    public override string ToString()
    {
        return $"{(ItemInstance != null ? ItemInstance.Name : "Empty")} ({Amount})";
    }
}