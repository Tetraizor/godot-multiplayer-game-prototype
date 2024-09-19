namespace Tetraizor.UI.Managers;

using System;
using Godot;
using Tetraizor.Autoloads;
using Tetraizor.UI.Components;
using Tetraizor.UI.Components.Modal;


public partial class LobbyUIManager : Control
{
    [Export] private Button _hostButton;
    [Export] private Button _joinButton;
    [Export] private Button _settingsButton;
    [Export] private Button _helpButton;
    [Export] private Button _quitButton;

    [Export] private Control _mainMenuPanel;

    #region State
    private bool _isMainMenuPanelVisible = true;
    #endregion

    public override void _Ready()
    {
        _hostButton.Pressed += OnHostButtonPressed;
        _joinButton.Pressed += OnJoinButtonPressed;
        _settingsButton.Pressed += OnSettingsButtonPressed;
        _helpButton.Pressed += OnHelpButtonPressed;

        _quitButton.Pressed += () => GetTree().Quit();
    }

    public void ToggleMainMenuPanel(bool state)
    {
        if (state == _isMainMenuPanelVisible) return;
        _isMainMenuPanelVisible = state;

        if (state)
        {
            var tween = GetTree().CreateTween();
            tween.SetEase(Tween.EaseType.In);
            tween.TweenProperty(
                _mainMenuPanel,
                "position",
                new Vector2(90, _mainMenuPanel.Position.Y),
                .2f
            );
            tween.Play();
        }
        else
        {
            var tween = GetTree().CreateTween();
            tween.SetEase(Tween.EaseType.In);
            tween.TweenProperty(
                _mainMenuPanel,
                "position",
                new Vector2(-600, _mainMenuPanel.Position.Y),
                .2f
            );
            tween.Play();
        }
    }

    public void ToggleMainMenuPanel()
    {
        ToggleMainMenuPanel(!_isMainMenuPanelVisible);
    }

    #region Callbacks
    public void OnJoinButtonPressed()
    {
        ToggleMainMenuPanel(false);
        NodeManager.FindNodeOfType<JoinModal>().Toggle();
    }

    private void OnHostButtonPressed()
    {
        ToggleMainMenuPanel(false);
        NodeManager.FindNodeOfType<HostModal>().Toggle();
    }

    private void OnHelpButtonPressed()
    {
        throw new NotImplementedException();
    }

    private void OnSettingsButtonPressed()
    {
        throw new NotImplementedException();
    }
    #endregion
}
