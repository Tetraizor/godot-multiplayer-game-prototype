namespace Tetraizor.Entity;

using Godot.Collections;
using Godot;
using Tetraizor.Interface.Entity;
using Tetraizor.Enums;
using Tetraizor.Utils;
using System;


public partial class TreeEntity : StaticEntityBase, IHittable
{
    [Export] private AnimationTree _animationTree;
    [Export] private Sprite2D _leaves;
    private string _treeType;

    public override void _Ready()
    {
        base._Ready();
    }

    public override Dictionary Serialize()
    {
        base.Serialize();

        Dictionary rawData = new Dictionary
        {
            { "network_id", EntityId },
            {"tree_type", _treeType}
        };

        return DictionaryUtils.Merge(base.Serialize(), rawData);
    }

    public override void Deserialize(Dictionary rawData)
    {
        base.Deserialize(rawData);

        RandomNumberGenerator rng = new RandomNumberGenerator();

        if (rawData.ContainsKey("tree_type"))
            _treeType = (string)rawData["tree_type"];
        else
            _treeType = rng.RandiRange(0, 1) == 0 ? "no_leaves" : "normal";

        if (_treeType == "no_leaves")
        {
            _leaves.Visible = false;
        }
    }

    public void Hit(IEntity sender, int damage, DamageType damageType)
    {
        _animationTree.Set("parameters/Tree/Hit/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
    }
}
