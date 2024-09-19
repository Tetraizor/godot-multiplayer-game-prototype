namespace Tetraizor.UI.Components.Modal;

using Godot;

public abstract partial class ModalBase : Control
{
    [ExportGroup("Base References")]
    [Export] private Button _closeButton;

    [Export] private Panel _background;
    [Export] private Control _panel;

    private bool _isOn = false;
    public bool IsOn => _isOn;

    public override void _Ready()
    {
        _closeButton.Pressed += OnCloseButtonPressed;

        Toggle(false, true);
    }

    protected virtual void OnCloseButtonPressed()
    {
        Toggle(false);
    }

    public void Toggle()
    {
        Toggle(!_isOn);
    }

    public void Toggle(bool state, bool force = false)
    {
        if (!force && _isOn == state) return;
        _isOn = state;

        if (state)
        {
            _background.MouseFilter = MouseFilterEnum.Stop;

            var stylebox = _background.GetThemeStylebox("panel") as StyleBoxFlat;

            var tween = GetTree().CreateTween();
            tween.SetEase(Tween.EaseType.InOut);
            tween.SetParallel(true);

            tween.TweenProperty(
                _panel,
                "anchor_top",
                0.5f,
                .2f
            );
            tween.TweenProperty(
                _panel,
                "anchor_bottom",
                0.5f,
                .2f
            );
            tween.TweenMethod(
                Callable.From((float progress) =>
                {
                    stylebox.BgColor = new Color(0, 0, 0, .5f * progress);
                }), 0f, 1f, .1f
            );

            tween.Play();
        }
        else
        {
            _background.MouseFilter = MouseFilterEnum.Ignore;

            var stylebox = _background.GetThemeStylebox("panel") as StyleBoxFlat;

            var tween = GetTree().CreateTween();
            tween.SetEase(Tween.EaseType.InOut);
            tween.SetParallel(true);

            tween.TweenProperty(
                _panel,
                "anchor_top",
                -0.5f,
                .2f
            );
            tween.TweenProperty(
                _panel,
                "anchor_bottom",
                -0.5f,
                .2f
            );
            tween.TweenMethod(
                Callable.From((float progress) =>
                {
                    stylebox.BgColor = new Color(0, 0, 0, .5f * (1 - progress));
                }), 0f, 1f, .1f
            );

            tween.Play();
        }
    }
}
