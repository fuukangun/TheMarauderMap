using Microsoft.Xna.Framework;

namespace TheMarauderMap.UI;

public sealed class MapContentViewportState
{
    public const float MinZoom = 1f;
    public const float MaxZoom = 3f;

    private Rectangle _contentBounds;
    private Rectangle _mapCoordinateBounds;

    public MapContentViewportState(Rectangle contentBounds)
        : this(contentBounds, new Rectangle(0, 0, contentBounds.Width, contentBounds.Height))
    {
    }

    public MapContentViewportState(Rectangle contentBounds, Rectangle mapCoordinateBounds)
    {
        _contentBounds = contentBounds;
        _mapCoordinateBounds = mapCoordinateBounds;
        Zoom = MinZoom;
        PanOffset = Vector2.Zero;
    }

    public float Zoom { get; private set; }
    public Vector2 PanOffset { get; private set; }
    public Rectangle ContentBounds => _contentBounds;
    public Rectangle MapCoordinateBounds => _mapCoordinateBounds;
    public Vector2 Anchor => new(
        _mapCoordinateBounds.X + _mapCoordinateBounds.Width / 2f,
        _mapCoordinateBounds.Y + _mapCoordinateBounds.Height / 2f);
    public bool IsZoomed => Zoom > MinZoom + 0.001f;

    public void ZoomBy(int wheelDirection)
    {
        if (wheelDirection == 0)
            return;

        int steps = Math.Max(1, Math.Abs(wheelDirection) / 120);
        int direction = Math.Sign(wheelDirection);
        for (int i = 0; i < steps; i++)
            ZoomStep(direction);
    }

    private void ZoomStep(int direction)
    {
        float oldZoom = Zoom;
        float newZoom = MathHelper.Clamp(Zoom + direction * 0.1f, MinZoom, MaxZoom);
        if (Math.Abs(newZoom - oldZoom) < 0.001f)
            return;

        Zoom = newZoom;
        ClampPan();
    }

    public void Pan(Vector2 contentDelta)
    {
        if (!IsZoomed)
            return;

        PanOffset += contentDelta;
        ClampPan();
    }

    public void PanScreenDelta(Vector2 screenDelta)
    {
        if (!IsZoomed)
            return;

        Pan(-screenDelta / Zoom);
    }

    public void SetContentBounds(Rectangle contentBounds)
    {
        SetContentBounds(contentBounds, _mapCoordinateBounds);
    }

    public void SetContentBounds(Rectangle contentBounds, Rectangle mapCoordinateBounds)
    {
        if (contentBounds.Width <= 0 || contentBounds.Height <= 0)
        {
            if (mapCoordinateBounds.Width > 0 && mapCoordinateBounds.Height > 0 && mapCoordinateBounds != _mapCoordinateBounds)
            {
                _mapCoordinateBounds = mapCoordinateBounds;
                Zoom = MinZoom;
                PanOffset = Vector2.Zero;
            }
            return;
        }

        bool contentSizeChanged = contentBounds.Width != _contentBounds.Width || contentBounds.Height != _contentBounds.Height;
        bool mapBoundsChanged = mapCoordinateBounds.Width > 0
            && mapCoordinateBounds.Height > 0
            && mapCoordinateBounds != _mapCoordinateBounds;

        _contentBounds = contentBounds;
        if (mapCoordinateBounds.Width > 0 && mapCoordinateBounds.Height > 0)
            _mapCoordinateBounds = mapCoordinateBounds;

        if (contentSizeChanged || mapBoundsChanged)
        {
            Zoom = MinZoom;
            PanOffset = Vector2.Zero;
        }
        else
        {
            ClampPan();
        }
    }

    private void ClampPan()
    {
        if (Zoom <= MinZoom + 0.001f)
        {
            Zoom = MinZoom;
            PanOffset = Vector2.Zero;
            return;
        }

        Vector2 contentSize = ContentSize;
        float maxX = (contentSize.X - contentSize.X / Zoom) / 2f;
        float maxY = (contentSize.Y - contentSize.Y / Zoom) / 2f;
        PanOffset = new Vector2(
            MathHelper.Clamp(PanOffset.X, -maxX, maxX),
            MathHelper.Clamp(PanOffset.Y, -maxY, maxY));
    }

    private Vector2 ContentSize => new(_mapCoordinateBounds.Width, _mapCoordinateBounds.Height);
}
