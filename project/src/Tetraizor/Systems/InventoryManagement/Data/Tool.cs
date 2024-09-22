using Tetraizor.Enums;

namespace Tetraizor.Systems.InventoryManagement.Data;

public class Tool : Item
{
    public int Range { get; private set; }
    public int Damage { get; private set; }
    public int EffectiveAngle { get; private set; }
    public ToolType Type { get; private set; }

    public Tool(int itemId, string name, string description, string texturePath, int range, int damage, int effectiveAngle, ToolType type) : base(itemId, name, description, 1, texturePath)
    {
        Range = range;
        Damage = damage;
        Type = type;
        EffectiveAngle = effectiveAngle;
    }
}