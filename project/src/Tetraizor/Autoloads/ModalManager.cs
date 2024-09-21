namespace Tetraizor.Autoloads;

using Godot;
using System.Collections.Generic;
using System.Linq;
using Tetraizor.UI.Modals;

public partial class ModalManager : AutoloadBase<ModalManager>
{
    [Export] private Control _background;
    [Export] private float _backgroundToggleTime = .1f;

    private List<ModalBase> _modals = new();
    public List<ModalBase> Modals => _modals;

    private List<ModalBase> _openModals = new();
    public List<ModalBase> OpenModals => _openModals;

    [Signal] public delegate void ModalStateChangedEventHandler(ModalBase modal, bool state);

    public static ModalBase GetModalByType<T>() where T : ModalBase
    {
        // var modal = Instance._modals.Find(modal => modal is T);
        ModalBase modal = null;
        if (modal != null)
        {
            return modal;
        }
        else
        {
            modal = NodeManager.FindNodeOfType<T>();

            // Cache modal.
            if (modal != null)
            {
                Instance._modals.Add(modal);
                return modal;
            }
        }

        return null;
    }

    public override void _Ready()
    {
        base._Ready();
        ToggleBackground(false);
    }

    public static void ToggleModal<T>() where T : ModalBase => ToggleModal(GetModalByType<T>());
    public static void ToggleModal(ModalBase modal)
    {
        if (modal == null) return;
        bool isModalOn = modal.IsOn;

        ToggleModal(modal, !isModalOn);
    }

    public static void ToggleModal<T>(bool state) where T : ModalBase => ToggleModal(GetModalByType<T>(), state);
    public static void ToggleModal(ModalBase modal, bool state)
    {
        if (modal == null) return;
        if (state == modal.IsOn) return;

        modal.Toggle(state);
        Instance.EmitSignal(nameof(ModalStateChanged), modal, state);

        if (state)
        {
            Instance._openModals.Add(modal);

            if (Instance._openModals.Count == 1)
            {
                Instance.ToggleBackground(true);
            }

            var openModals = new List<ModalBase>(Instance._openModals);
            foreach (ModalBase openModal in openModals)
            {
                if (openModal.ModalLayer != modal.ModalLayer)
                {
                    ToggleModal(openModal, false);
                }
            }
        }
        else
        {
            Instance._openModals.Remove(modal);

            if (Instance._openModals.Count == 0)
            {
                // Means this was the last modal open, toggle background off.
                Instance.ToggleBackground(false);
            }
        }
    }

    private void ToggleBackground(bool state)
    {
        var stylebox = (StyleBoxFlat)_background.GetThemeStylebox("panel");

        _background.MouseFilter = state ? Control.MouseFilterEnum.Stop : Control.MouseFilterEnum.Ignore;

        var tween = GetTree().CreateTween();
        tween.TweenMethod(Callable.From((float progress) =>
        {
            stylebox.BgColor = new Color(0, 0, 0, .6f * (state ? progress : 1 - progress));

        }), 0f, 1f, _backgroundToggleTime);
    }

    public static void CloseModals()
    {
        var modalsToClose = new List<ModalBase>(Instance._openModals);
        foreach (ModalBase openModal in modalsToClose)
        {
            ToggleModal(openModal, false);
        }
    }

    public static void RemoveModal(ModalBase modal)
    {
        Instance._openModals.Remove(modal);
        Instance._modals.Remove(modal);

        if (Instance._openModals.Count == 0)
        {
            Instance.ToggleBackground(false);
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("ui_close_menu"))
        {
            if (_openModals.Count > 0)
            {
                GetTree().Root.SetInputAsHandled();
                CloseModals();
            }
        }
    }
}
