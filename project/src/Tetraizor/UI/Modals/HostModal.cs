namespace Tetraizor.UI.Modals;

using Godot;
using Tetraizor.Autoloads;
using Tetraizor.Data.Networking;
using Tetraizor.Utils.Network;
using Tetrazior.Data.Networking;

public partial class HostModal : ModalBase
{
    [ExportGroup("Host Control References")]
    [Export] private Button _hostButton;

    [Export] private LineEdit _ipAddressLineEdit;
    [Export] private LineEdit _portLineEdit;
    [Export] private LineEdit _usernameLineEdit;

    public override void _Ready()
    {
        base._Ready();

        _ipAddressLineEdit.TextChanged += (string text) => CheckForm();
        _portLineEdit.TextChanged += (string text) => CheckForm();
        _usernameLineEdit.TextChanged += (string text) => CheckForm();

        _hostButton.Pressed += OnHostButtonPressed;
    }

    private void CheckForm()
    {
        var ipAddress = _ipAddressLineEdit.Text;
        var port = _portLineEdit.Text;
        var username = _usernameLineEdit.Text;

        bool ipCheck = NetworkUtils.IsValidIpAddress(ipAddress) || NetworkUtils.IsValidDomain(ipAddress);
        bool portCheck = NetworkUtils.IsValidPort(port);
        bool usernameCheck = !string.IsNullOrEmpty(username);

        _hostButton.Disabled = !(ipCheck && portCheck && usernameCheck);
    }

    private void OnHostButtonPressed()
    {
        var ipAddress = _ipAddressLineEdit.Text;
        var port = _portLineEdit.Text;
        var username = _usernameLineEdit.Text;

        if (!NetworkUtils.IsValidIpAddress(ipAddress))
        {
            var hostEntry = System.Net.Dns.GetHostEntry(ipAddress);

            ipAddress = hostEntry.AddressList[0].ToString();
        }

        NetworkManager.Instance.Host(
            new ServerProperties
            {
                Ip = ipAddress,
                Port = int.Parse(port)
            },
            new UserData
            {
                Username = username
            }
        );

        ModalManager.ToggleModal(this, false);
    }
}
