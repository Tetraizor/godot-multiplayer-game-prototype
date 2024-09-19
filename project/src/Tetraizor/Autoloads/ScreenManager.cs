using Godot;
using System;

public partial class ScreenManager : Node
{
    public static bool IsFocused { get; private set; } = true;

    public override void _Ready()
    {
        GetViewport().SizeChanged += OnScreenSizeChanged;

        GetWindow().FocusEntered += OnWindowFocusEntered;
        GetWindow().FocusExited += OnWindowFocusExited;
    }

    private void OnWindowFocusExited()
    {
        IsFocused = false;
    }

    private void OnWindowFocusEntered()
    {
        IsFocused = true;
    }

    private void OnScreenSizeChanged()
    {
    }
}
