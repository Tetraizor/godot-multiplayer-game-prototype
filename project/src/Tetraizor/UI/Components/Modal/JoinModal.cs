namespace Tetraizor.UI.Components.Modal;

using Godot;
using Tetraizor.Autoloads;
using Tetraizor.UI.Managers;
using Tetraizor.Utils.Network;
using Tetrazior.Data.Networking;

public partial class JoinModal : ModalBase
{
    [ExportGroup("Join Control References")]
    [Export] private Button _joinButton;

    [Export] private LineEdit _ipAddressLineEdit;
    [Export] private LineEdit _portLineEdit;
    [Export] private LineEdit _usernameLineEdit;

    private LobbyUIManager _lobbyUIManager;

    public override void _Ready()
    {
        base._Ready();

        _lobbyUIManager = NodeManager.FindNodeOfType<LobbyUIManager>();

        _ipAddressLineEdit.TextChanged += (string text) => CheckForm();
        _portLineEdit.TextChanged += (string text) => CheckForm();
        _usernameLineEdit.TextChanged += (string text) => CheckForm();

        _joinButton.Pressed += OnJoinButtonPressed;
    }

    protected override void OnCloseButtonPressed()
    {
        base.OnCloseButtonPressed();

        _lobbyUIManager.ToggleMainMenuPanel(true);
    }

    private void CheckForm()
    {
        var ipAddress = _ipAddressLineEdit.Text;
        var port = _portLineEdit.Text;
        var username = _usernameLineEdit.Text;

        bool ipCheck = NetworkUtils.IsValidIpAddress(ipAddress) || NetworkUtils.IsValidDomain(ipAddress);
        bool portCheck = NetworkUtils.IsValidPort(port);
        bool usernameCheck = !string.IsNullOrEmpty(username);

        _joinButton.Disabled = !(ipCheck && portCheck && usernameCheck);
    }

    private void OnJoinButtonPressed()
    {
        var ipAddress = _ipAddressLineEdit.Text;
        var port = _portLineEdit.Text;
        var username = _usernameLineEdit.Text;

        if (!NetworkUtils.IsValidIpAddress(ipAddress))
        {
            var hostEntry = System.Net.Dns.GetHostEntry(ipAddress);

            ipAddress = hostEntry.AddressList[0].ToString();
        }

        NetworkManager.Instance.Join(
            new UserData
            {
                Username = username
            },
            ipAddress, int.Parse(port)
        );
    }
}
