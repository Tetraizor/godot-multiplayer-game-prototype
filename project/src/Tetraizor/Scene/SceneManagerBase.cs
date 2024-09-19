namespace Tetraizor.Scene;

using Godot;
using Tetraizor.Autoloads;

public abstract partial class SceneManagerBase : Node
{
    public override void _Ready()
    {
        TransitionManager.EndTransition();
    }
}