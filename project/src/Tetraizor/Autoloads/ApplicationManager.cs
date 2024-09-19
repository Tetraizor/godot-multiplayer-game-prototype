using System;
using Godot;
using Tetraizor.Autoloads;

namespace Tetraizor.Autoloads;

public partial class ApplicationManager : AutoloadBase<ApplicationManager>
{
    public Vector2I ScreenSize => _screenSize;
    private Vector2I _screenSize;

    [Signal] public delegate void ScreenSizeChangedEventHandler(Vector2I newSize);

    public override void _Ready()
    {
        base._Ready();

        GetViewport().SizeChanged += OnScreenSizeChanged;
        OnScreenSizeChanged();
    }

    private void OnScreenSizeChanged()
    {
        var newSize = new Vector2I(GetViewport().GetWindow().Size.X, GetViewport().GetWindow().Size.Y);
        _screenSize = newSize;

        EmitSignal(
            SignalName.ScreenSizeChanged,
            _screenSize
        );
    }
}