namespace Tetraizor.Autoloads;

using Godot;
using Godot.Collections;
using Tetraizor.Data.Networking.Packet;
using Tetraizor.Enums;

public partial class CM : AutoloadBase<CM>
{
    [Export] private RichTextLabel _consoleOutput;
    [Export] private LineEdit _consoleInput;
    [Export] private Button _sendButton;

    [Export] private Control _consoleContainer;
    [Export] private MarginContainer _content;
    [Export] private Panel _background;

    private bool _isOn;
    public bool IsOn
    {
        get => _isOn;
        set
        {
            _isOn = value;
            _content.Visible = value;

            if (_isOn)
            {
                var tween = GetTree().CreateTween();
                var stylebox = _background.GetThemeStylebox("panel") as StyleBoxFlat;
                _background.MouseFilter = Control.MouseFilterEnum.Stop;

                tween.TweenMethod(Callable.From((float progress) =>
                {
                    stylebox.BgColor = new Color(0, 0, 0, progress * .5f);
                }), 0f, 1f, .05f);
                tween.Play();
            }
            else
            {
                var tween = GetTree().CreateTween();
                var stylebox = _background.GetThemeStylebox("panel") as StyleBoxFlat;
                _background.MouseFilter = Control.MouseFilterEnum.Ignore;

                tween.TweenMethod(Callable.From((float progress) =>
                {
                    stylebox.BgColor = new Color(0, 0, 0, (1 - progress) * .5f);
                }), 0f, 1f, .05f);
                tween.Play();
            }
        }
    }

    public override void _Ready()
    {
        base._Ready();

        _sendButton.Pressed += OnSendButtonPressed;
        PacketBus.Instance.PacketReceived += OnPacketReceived;
    }

    public override void _ExitTree()
    {
        base._ExitTree();

        PacketBus.Instance.PacketReceived -= OnPacketReceived;
    }

    private void OnPacketReceived(int senderId, PacketType type, Dictionary rawData)
    {
        if (type == PacketType.ChatMessage)
        {
            var packet = new ChatMessagePacket(PacketType.ChatMessage, "", "");
            packet.Deserialize(rawData);

            Print($"{packet.SenderName}: {packet.Message}");
        }
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (@event is InputEventKey keyEvent && keyEvent.Pressed)
        {
            switch (keyEvent.Keycode)
            {
                case Key.F1:
                    IsOn = !_isOn;
                    if (_isOn) _consoleInput.GrabFocus();
                    break;
                case Key.Enter:
                    if (_isOn && _consoleInput.HasFocus()) OnSendButtonPressed();
                    break;
                default:
                    break;
            }
        }
    }

    private void OnSendButtonPressed()
    {
        if (!_isOn) return;

        ProcessConsoleInput();
    }

    private void ProcessConsoleInput()
    {
        EnterText(_consoleInput.Text);
        _consoleInput.Clear();
    }

    public void EnterText(string text)
    {
        if (text == null || text == string.Empty) return;

        if (text.StartsWith("/"))
        {
            ProcessCommand(text);
        }
        else
        {
            if (NetworkManager.Instance.IsConnectionActive)
            {
                PacketBus.QueuePacket(new ChatMessagePacket(PacketType.ChatMessage, text, NetworkManager.LocalProfile.Username));
            }
            else
            {
                Print(text);
            }
        }
    }

    private void ProcessCommand(string text)
    {
        var command = text.Substring(1).ToLower();
        var args = command.Split(' ');

        switch (args[0])
        {
            case "help":
                Print("Help: /help");
                break;
            case "clear":
                _consoleOutput.Clear();
                break;
            default:
                Error($"Unknown command: {args[0]}");
                break;
        }
    }

    public static void Print(object message)
    {
        Instance._consoleOutput.Text += $"{message}\n";
        GD.Print(message);
    }

    public static void Warning(object message)
    {
        Instance._consoleOutput.Text += $"[color=yellow]{message}[/color]\n";
        GD.PrintRich($"[color=yellow]{message}[/color]");
    }

    public static void Error(object message)
    {
        Instance._consoleOutput.Text += $"[color=red]{message}[/color]\n";
        GD.PrintErr(message);
    }

    public static void Correct(object message)
    {
        Instance._consoleOutput.Text += $"[color=green]{message}[/color]\n";
        GD.PrintRich($"[color=green]{message}[/color]");
    }
}