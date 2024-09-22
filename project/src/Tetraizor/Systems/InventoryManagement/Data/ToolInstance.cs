namespace Tetraizor.Systems.InventoryManagement.Data;

using Godot;
using Godot.Collections;
using Tetraizor.Autoloads;
using Tetraizor.Utils;

public class ToolInstance : ItemInstance
{
    // TODO: Durability is fixed now, change it to get its max dur. from
    // TODO: Tool class too.
    public int Durability { get; private set; } = 100;
    public Tool Tool { get; private set; }

    public int Range => Tool.Range;
    public int EffectiveAngle => Tool.EffectiveAngle;
    public int Damage => Tool.Damage;

    public ToolInstance(int itemId) : base(itemId)
    {
        Tool = (Tool)Item;
    }

    public override void Deserialize(Dictionary rawData)
    {
        base.Deserialize(rawData);
        Tool = (Tool)Item;
    }

    public override Dictionary Serialize()
    {
        var serializedData = base.Serialize();

        return DictionaryUtils.Merge(serializedData, new Dictionary
        {
            // TODO: Add durability here.
        });
    }
}