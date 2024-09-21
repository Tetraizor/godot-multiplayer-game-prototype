namespace Tetraizor.UI.Modals;

using Godot;
using Tetraizor.Autoloads;

public partial class MainMenuModal : ModalBase
{
    [Export] private Button _disconnectButton;
    [Export] private Label _playerInfoLabel;

    public override void _Ready()
    {
        base._Ready();

        ModalLayer = 1;
        Visible = true;

        _disconnectButton.Pressed += OnDisconnectButtonPressed;
        _playerInfoLabel.Text = $"Player: {NetworkManager.LocalProfile.Username}";
    }

    private void OnDisconnectButtonPressed()
    {
        NetworkManager.Instance.Disconnect();
    }

    // TODO: Temp solution for input.
    public override void _UnhandledInput(InputEvent @event)
    {
        if (Input.IsActionJustPressed("ui_close_menu"))
        {
            if (!IsOn)
            {
                GetTree().Root.SetInputAsHandled();
                ModalManager.ToggleModal(this, true);
            }
        }
    }
}