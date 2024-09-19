using Godot;
using Tetraizor.Autoloads;

namespace Tetraizor.UI.Components.Modal;

public partial class MainMenuModal : ModalBase
{
    [Export] private Button _disconnectButton;
    [Export] private Label _playerInfoLabel;

    public override void _Ready()
    {
        base._Ready();

        Visible = true;

        _disconnectButton.Pressed += OnDisconnectButtonPressed;
        _playerInfoLabel.Text = $"Player: {NetworkManager.LocalProfile.Username}";
    }

    private void OnDisconnectButtonPressed()
    {
        NetworkManager.Instance.Disconnect();
    }

    // TODO: Temp solution for input.
    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (@event is InputEventKey keyEvent)
        {
            if (keyEvent.Pressed && keyEvent.Keycode == Key.Escape)
            {
                Toggle();
            }
        }
    }
}