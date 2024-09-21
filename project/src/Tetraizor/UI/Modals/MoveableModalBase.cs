namespace Tetraizor.UI.Modals;

using System;
using Godot;

public abstract partial class MoveableModalBase : ModalBase
{
    [Export] private Control _grabPart;
    [Export] private Control _moveable;

    private bool _isHolding = false;
    private bool _isHoveringOnHoldable = false;

    private Vector2 _screenSize = Vector2.Zero;

    public override void _Ready()
    {
        base._Ready();

        _grabPart.MouseEntered += OnHoldableMouseEntered;
        _grabPart.MouseExited += OnHoldableMouseExited;

        GetViewport().SizeChanged += OnViewportSizeChanged;
        OnViewportSizeChanged();
    }

    public override void _ExitTree()
    {
        base._ExitTree();

        _grabPart.MouseEntered -= OnHoldableMouseEntered;
        _grabPart.MouseExited -= OnHoldableMouseExited;

        GetViewport().SizeChanged -= OnViewportSizeChanged;
    }

    private void OnViewportSizeChanged()
    {
        _screenSize = GetViewport().GetVisibleRect().Size;

        if (IsOn) ValidatePosition();
    }

    private void OnHoldableMouseExited()
    {
        _isHoveringOnHoldable = false;
    }

    private void OnHoldableMouseEntered()
    {
        _isHoveringOnHoldable = true;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (@event is InputEventMouseButton @eventMouseButton)
        {
            if (_isHoveringOnHoldable && @eventMouseButton.Pressed && @eventMouseButton.ButtonIndex == MouseButton.Left)
            {
                _isHolding = true;
            }
            else
            {
                _isHolding = false;
            }
        }

        if (@event is InputEventMouseMotion @eventMouseMotion)
        {
            if (_isHolding)
            {
                _moveable.Position += @eventMouseMotion.Relative;
                ValidatePosition();
            }
        }
    }

    public void ValidatePosition()
    {
        if (_moveable.GlobalPosition.X < 0)
        {
            _moveable.GlobalPosition = new Vector2(0, _moveable.GlobalPosition.Y);
        }

        if ((_moveable.GlobalPosition.X + _moveable.Size.X) > _screenSize.X)
        {
            _moveable.GlobalPosition = new Vector2(_screenSize.X - _moveable.Size.X, _moveable.GlobalPosition.Y);
        }

        if (_moveable.GlobalPosition.Y < 0)
        {
            _moveable.GlobalPosition = new Vector2(_moveable.GlobalPosition.X, 0);
        }

        if ((_moveable.GlobalPosition.Y + _moveable.Size.Y) > _screenSize.Y)
        {
            _moveable.GlobalPosition = new Vector2(_moveable.GlobalPosition.X, _screenSize.Y - _moveable.Size.Y);
        }
    }
}