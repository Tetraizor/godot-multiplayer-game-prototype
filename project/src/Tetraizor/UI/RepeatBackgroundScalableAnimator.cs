namespace Tetraizor.UI.Helpers;

using Godot;
using Tetraizor.UI.Components;

public partial class RepeatBackgroundScalableAnimator : Node
{
    [Export] private RepeatBackgroundScalable _backgroundPanel;
    [Export] private Vector2 ScrollSpeed;

    public override void _Process(double delta)
    {
        if (_backgroundPanel == null) return;

        _backgroundPanel.Offset += ScrollSpeed * (float)delta;
    }
}
