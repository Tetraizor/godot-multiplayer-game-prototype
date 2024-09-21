namespace Tetraizor.Systems.InventoryManagement.Data;

using Godot;

public partial class InventorySlot : TextureRect
{
    [Export] private Label _countLabel;
    [Export] private TextureRect _itemDisplay;

    private Inventory _connectedInventory;

    public Item Item { get; private set; }
    public int Amount { get; private set; }

    private bool _isHovering;

    public override void _Ready()
    {
        ClearItem();

        MouseEntered += OnHoverStart;
        MouseExited += OnHoverEnd;
    }

    public void Setup(Inventory inventory, Item item = null, int amount = 0)
    {
        _connectedInventory = inventory;
        Item = item;
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

    public void SetItem(Item item, int amount = 1)
    {
        if (item == null)
        {
            ClearItem();
            return;
        }

        Amount = amount;
        Item = item;

        _itemDisplay.Texture = item.Sprite;
        _countLabel.Text = Amount > 1 ? Amount.ToString() : "";
    }

    public void ClearItem()
    {
        Amount = 0;
        Item = null;

        _itemDisplay.Texture = null;
        _countLabel.Text = "";
    }

    public void AddItem(int amount)
    {
        if (amount < 0 || Item == null || (Amount + amount) > Item.StackSize) return;

        Amount += amount;

        _countLabel.Text = Amount > 1 ? Amount.ToString() : "";
    }

    public void RemoveItem(int amount)
    {
        if (amount < 0 || Amount < amount || Item == null) return;
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
        return $"{(Item != null ? Item.Name : "Empty")} ({Amount})";
    }
}