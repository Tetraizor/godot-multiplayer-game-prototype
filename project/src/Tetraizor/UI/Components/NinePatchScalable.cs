using Godot;

namespace Tetraizor.UI.Components;

[Tool]
public partial class NinePatchScalable : Control
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

    private Rect2 _middleRect;
    [Export]
    public Rect2 MiddleRect
    {
        get => _middleRect;
        set
        {
            _middleRect = value;
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

    public override void _Draw()
    {
        base._Draw();
        if (_texture == null)
        {
            return;
        }

        if (_middleRect.Size == Vector2.Zero)
        {
            _middleRect.Size = new Vector2(_texture.GetWidth(), _texture.GetHeight());
            _middleRect.Position = Vector2.Zero;
        }

        if (_drawScale < 0)
        {
            GD.PrintErr("DrawScale cannot be less than 0.");
            _drawScale = 1;
        }

        float controlWidth = Size.X;
        float controlHeight = Size.Y;

        float middleWidth = controlWidth - ((_texture.GetSize().X - _middleRect.Size.X) * _drawScale);
        float middleHeight = controlHeight - (_texture.GetSize().Y - _middleRect.Size.Y) * _drawScale;

        /* What do the floats below represent?

                   h1                      h2       h3
           *----------------------------------------*
           |   tl  |                       |   tr   |
           |       |                       |        |
        v1 |-------|-----------------------|--------|
           |       |                       |        |
           |       |                       |        |
           |       |                       |        |
           |       |                       |        |
        v2 |-------|-----------------------|--------|
           |  bl   |                       |   br   |
           |       |                       |        |
        v3 *----------------------------------------*
                        (9-PatchRect)

        */

        float h1 = _middleRect.Position.X * _drawScale;
        float h2 = middleWidth + h1;
        float h3 = controlWidth;

        float v1 = _middleRect.Position.Y * _drawScale;
        float v2 = middleHeight + v1;
        float v3 = controlHeight;

        // Top Left
        float x = 0;
        float y = 0;
        float w = _middleRect.Position.X;
        float h = _middleRect.Position.Y;

        DrawTextureRectRegion(_texture,
            new Rect2(0, 0, h1, v1),
            new Rect2(x, y, w, h));

        // Top Right
        x = _middleRect.Position.X + _middleRect.Size.X;
        // y is the same.
        w = _texture.GetWidth() - x;
        // h is the same.

        DrawTextureRectRegion(_texture,
            new Rect2(h2, 0, h3 - h2, v1),
            new Rect2(x, y, w, h));

        // Bottom Right
        // x is the same.
        y = _middleRect.Position.Y + _middleRect.Size.Y;
        // w is the same.
        h = _texture.GetHeight() - y;
        DrawTextureRectRegion(_texture,
            new Rect2(h2, v2, h3 - h2, v3 - v2),
            new Rect2(x, y, w, h));

        // Bottom Left
        x = 0;
        y = _middleRect.Position.Y + _middleRect.Size.Y;
        w = _middleRect.Position.X;
        // h is the same.

        DrawTextureRectRegion(_texture,
            new Rect2(0, v2, h1, v3 - v2),
            new Rect2(x, y, w, h));

        // Horizontal
        float hMiddleNeed = middleWidth / (_middleRect.Size.X * _drawScale);
        float vMiddleNeed = middleHeight / (_middleRect.Size.Y * _drawScale);

        int hMiddleCount = Mathf.Max(Mathf.RoundToInt(hMiddleNeed), 1);
        int vMiddleCount = Mathf.Max(Mathf.RoundToInt(vMiddleNeed), 1);

        float xScale = hMiddleNeed / (float)hMiddleCount;
        float yScale = vMiddleNeed / (float)vMiddleCount;

        for (int xCount = 0; xCount < hMiddleCount; xCount++)
        {
            float xSize = xScale * _middleRect.Size.X * _drawScale;
            float currentX = h1 + (xCount * xSize);

            DrawTextureRectRegion(_texture,
                new Rect2(currentX, 0, xSize, v1),
                new Rect2(_middleRect.Position.X, 0, _middleRect.Size.X, _middleRect.Position.Y));

            DrawTextureRectRegion(_texture,
                new Rect2(currentX, v2, xSize, v3 - v2),
                new Rect2(_middleRect.Position.X, _middleRect.Position.Y + _middleRect.Size.Y, _middleRect.Size.X, _texture.GetSize().Y - (_middleRect.Position.Y + _middleRect.Size.Y)));
        }

        for (int yCount = 0; yCount < vMiddleCount; yCount++)
        {
            float ySize = yScale * _middleRect.Size.Y * _drawScale;
            float currentY = v1 + (yCount * ySize);

            DrawTextureRectRegion(_texture,
                new Rect2(0, currentY, h1, ySize),
                new Rect2(0, _middleRect.Position.Y, _middleRect.Position.X, _middleRect.Size.Y));

            DrawTextureRectRegion(_texture,
                new Rect2(h2, currentY, h3 - h2, ySize),
                new Rect2(_middleRect.Position.X + _middleRect.Size.X, _middleRect.Position.Y, _texture.GetWidth() - (_middleRect.Position.X + _middleRect.Size.X), _middleRect.Size.Y));
        }

        for (int xMidCount = 0; xMidCount < hMiddleCount; xMidCount++)
        {
            for (int yMidCount = 0; yMidCount < vMiddleCount; yMidCount++)
            {
                float xSize = xScale * _middleRect.Size.X * _drawScale;
                float ySize = yScale * _middleRect.Size.Y * _drawScale;

                float currentX = h1 + (xMidCount * xSize);
                float currentY = v1 + (yMidCount * ySize);

                DrawTextureRectRegion(_texture,
                    new Rect2(currentX, currentY, xSize, ySize),
                    new Rect2(_middleRect.Position.X, _middleRect.Position.Y, _middleRect.Size.X, _middleRect.Size.Y));
            }
        }

        // Debug Lines
        // DrawLine(new Vector2(h1, 0), new Vector2(h1, v3), Colors.Red, 2);
        // DrawLine(new Vector2(h2, 0), new Vector2(h2, v3), Colors.Red, 2);

        // DrawLine(new Vector2(0, v1), new Vector2(h3, v1), Colors.Red, 2);
        // DrawLine(new Vector2(0, v2), new Vector2(h3, v2), Colors.Red, 2);
    }
}
