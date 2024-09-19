namespace Tetraizor.Autoloads;

using System;
using Godot;

public partial class TransitionManager : AutoloadBase<TransitionManager>
{
    [Export] private Panel _transitionPanel;

    [Export] private float _duration = 0.3f;
    [Export] private float _waitTime = 0.3f;

    [Signal] public delegate void StartFinishedEventHandler();
    [Signal] public delegate void EndFinishedEventHandler();

    public static void StartTransition()
    {
        Instance._transitionPanel.MouseFilter = Control.MouseFilterEnum.Stop;

        var stylebox = Instance._transitionPanel.GetThemeStylebox("panel") as StyleBoxFlat;

        var tween = Instance.GetTree().CreateTween();
        tween.SetEase(Tween.EaseType.InOut);
        tween.Finished += () => Instance.EmitSignal(SignalName.StartFinished);

        tween.TweenMethod(Callable.From((float progress) =>
        {
            stylebox.BgColor = new Color(0, 0, 0, progress);
        }), 0f, 1f, Instance._duration);

        tween.TweenInterval(Instance._waitTime);
    }

    public static void EndTransition()
    {
        Instance._transitionPanel.MouseFilter = Control.MouseFilterEnum.Ignore;

        var stylebox = Instance._transitionPanel.GetThemeStylebox("panel") as StyleBoxFlat;

        var tween = Instance.GetTree().CreateTween();
        tween.SetEase(Tween.EaseType.InOut);
        tween.Finished += () => Instance.EmitSignal(SignalName.EndFinished);

        tween.TweenMethod(Callable.From((float progress) =>
        {
            stylebox.BgColor = new Color(0, 0, 0, 1 - progress);
        }), 0f, 1f, Instance._duration);
    }
}
