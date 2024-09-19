namespace Tetraizor.UI.Components;

using Godot;

[Tool]
public partial class RepeatBackgroundScalable : Control
{
    private Texture2D _texture;

    /// <summary>
    /// The texture to be drawn.
    /// </summary>
    [Export]
    private Texture2D Texture
    {
        get => _texture;
        set
        {
            _texture = value;
            this.QueueRedraw();
        }
    }

    private float _drawScale = 1f;
    [Export]
    public float DrawScale
    {
        get => _drawScale;
        set
        {
            _drawScale = value;
            this.QueueRedraw();
        }
    }

    private Vector2 _offset = Vector2.Zero;
    [Export]
    public Vector2 Offset
    {
        get => _offset;
        set
        {
            _offset = value;
            this.QueueRedraw();
        }
    }

    public override void _Draw()
    {
        base._Draw();

        if (_texture == null || _drawScale <= 0.05f)
        {
            return;
        }

        var textureSize = new Vector2(_texture.GetWidth(), _texture.GetHeight());
        var panelSize = Size;
        var scaledSize = textureSize * _drawScale;
        var count = new Vector2I(
            Mathf.RoundToInt(panelSize.X / scaledSize.X) + 1,
            Mathf.RoundToInt(panelSize.Y / scaledSize.Y) + 1
        );

        var repeatedOffset = new Vector2(
            _offset.X % textureSize.X,
            _offset.Y % textureSize.Y
        ) * _drawScale;

        for (int x = -1; x < count.X; x++)
        {
            for (int y = -1; y < count.Y; y++)
            {
                DrawTextureRect(
                    _texture,
                    new Rect2(
                        new Vector2(repeatedOffset.X + x * scaledSize.X, repeatedOffset.Y + y * scaledSize.Y),
                        new Vector2(scaledSize.X, scaledSize.Y)),
                    false,
                    Modulate
                );
            }
        }
    }
}