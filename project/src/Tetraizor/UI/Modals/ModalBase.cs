namespace Tetraizor.UI.Modals;

using Godot;
using Tetraizor.Autoloads;
using Tetraizor.Extensions;

public abstract partial class ModalBase : Control
{
    [ExportGroup("Base References")]
    [Export] private Button _closeButton;
    [Export] protected Control _panel;

    private bool _isOn = false;
    public bool IsOn => _isOn;

    public int ModalLayer { protected set; get; } = 0;

    public override void _Ready()
    {
        if (_closeButton != null) _closeButton.Pressed += OnCloseButtonPressed;
        if (_panel == null) _panel = this;
        _panel.Visible = true;

        SceneManager.Instance.SceneUnloaded += OnSceneUnloaded;
        Toggle(false, true);
    }

    private void OnSceneUnloaded(int sceneId, Node sceneRoot)
    {
        if (IsOn && this.IsParent(sceneRoot))
        {
            ModalManager.RemoveModal(this);
        }
    }

    protected virtual void OnCloseButtonPressed()
    {
        ModalManager.ToggleModal(this, false);
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

            tween.Play();
        }
        else
        {
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

            tween.Play();
        }
    }
}
