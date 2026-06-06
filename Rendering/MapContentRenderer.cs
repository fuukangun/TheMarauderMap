using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheMarauderMap.Rendering;

public sealed class MapContentRenderer : IDisposable
{
    private Texture2D? _snapshotTexture;
    private Rectangle? _snapshotBounds;
    public Rectangle? LastSourceRectangle { get; private set; }
    public string TextureStatus { get; private set; } = "snapshot none";

    public void Draw(SpriteBatch spriteBatch, Rectangle contentBounds, Vector2 panOffset, float zoom)
    {
        spriteBatch.End();
        CaptureSnapshot(spriteBatch.GraphicsDevice, contentBounds);
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

        Texture2D? texture = _snapshotTexture;
        if (texture is null)
        {
            LastSourceRectangle = null;
            return;
        }

        Vector2 texturePanOffset = new(
            panOffset.X * texture.Width / Math.Max(1f, contentBounds.Width),
            panOffset.Y * texture.Height / Math.Max(1f, contentBounds.Height));

        LastSourceRectangle = MapContentSourceRect.ForViewport(
            new Rectangle(0, 0, texture.Width, texture.Height),
            texturePanOffset,
            zoom);

        spriteBatch.Draw(texture, contentBounds, LastSourceRectangle.Value, Color.White);
    }

    public void Dispose()
    {
        _snapshotTexture?.Dispose();
        _snapshotTexture = null;
        _snapshotBounds = null;
    }

    private void CaptureSnapshot(GraphicsDevice graphicsDevice, Rectangle contentBounds)
    {
        if (contentBounds.Width <= 0 || contentBounds.Height <= 0)
            return;

        try
        {
            if (_snapshotTexture is null || _snapshotBounds != contentBounds)
            {
                _snapshotTexture?.Dispose();
                _snapshotTexture = new Texture2D(graphicsDevice, contentBounds.Width, contentBounds.Height);
                _snapshotBounds = contentBounds;
            }

            PresentationParameters pp = graphicsDevice.PresentationParameters;
            int backBufferWidth = pp.BackBufferWidth;
            int backBufferHeight = pp.BackBufferHeight;
            Rectangle captureBounds = ToBackBufferBounds(graphicsDevice, contentBounds);
            Color[] backBufferPixels = new Color[backBufferWidth * backBufferHeight];
            graphicsDevice.GetBackBufferData(backBufferPixels);

            Color[] snapshotPixels = new Color[contentBounds.Width * contentBounds.Height];
            for (int y = 0; y < contentBounds.Height; y++)
            {
                int sourceY = captureBounds.Y + (int)MathF.Round(y * captureBounds.Height / (float)contentBounds.Height);
                if (sourceY < 0 || sourceY >= backBufferHeight)
                    continue;

                for (int x = 0; x < contentBounds.Width; x++)
                {
                    int sourceX = captureBounds.X + (int)MathF.Round(x * captureBounds.Width / (float)contentBounds.Width);
                    if (sourceX < 0 || sourceX >= backBufferWidth)
                        continue;

                    int destinationY = contentBounds.Height - 1 - y;
                    snapshotPixels[destinationY * contentBounds.Width + x] = backBufferPixels[sourceY * backBufferWidth + sourceX];
                }
            }

            _snapshotTexture.SetData(snapshotPixels);
            TextureStatus = $"snapshot {_snapshotTexture.Width}x{_snapshotTexture.Height} cap={captureBounds}";
        }
        catch (Exception ex)
        {
            LastSourceRectangle = null;
            TextureStatus = $"snapshot failed: {ex.GetType().Name}";
        }
    }

    private static Rectangle ToBackBufferBounds(GraphicsDevice graphicsDevice, Rectangle uiBounds)
    {
        PresentationParameters pp = graphicsDevice.PresentationParameters;
        Rectangle viewport = graphicsDevice.Viewport.Bounds;
        float scaleX = viewport.Width / Math.Max(1f, (float)StardewValley.Game1.uiViewport.Width);
        float scaleY = viewport.Height / Math.Max(1f, (float)StardewValley.Game1.uiViewport.Height);
        int x = viewport.X + (int)MathF.Round(uiBounds.X * scaleX);
        int y = viewport.Y + (int)MathF.Round(uiBounds.Y * scaleY);
        int width = Math.Max(1, (int)MathF.Round(uiBounds.Width * scaleX));
        int height = Math.Max(1, (int)MathF.Round(uiBounds.Height * scaleY));
        return new Rectangle(
            Math.Clamp(x, 0, Math.Max(0, pp.BackBufferWidth - 1)),
            Math.Clamp(y, 0, Math.Max(0, pp.BackBufferHeight - 1)),
            Math.Min(width, Math.Max(1, pp.BackBufferWidth - x)),
            Math.Min(height, Math.Max(1, pp.BackBufferHeight - y)));
    }
}
