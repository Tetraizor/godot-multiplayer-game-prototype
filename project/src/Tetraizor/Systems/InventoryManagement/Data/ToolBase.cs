namespace Tetraizor.Systems.InventoryManagement.Data;

public abstract class ToolBase : Item
{
    protected ToolBase(int itemId, string name, string description) : base(itemId, name, description, 1)
    {

    }
}