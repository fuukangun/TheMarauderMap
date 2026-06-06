using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheMarauderMap.Rendering;

public readonly struct SpriteBatchScissorScope : IDisposable
{
    private readonly SpriteBatch _spriteBatch;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly Rectangle _previousScissorRectangle;

    private SpriteBatchScissorScope(SpriteBatch spriteBatch, Rectangle scissorRectangle)
    {
        _spriteBatch = spriteBatch;
        _graphicsDevice = spriteBatch.GraphicsDevice;
        _previousScissorRectangle = _graphicsDevice.ScissorRectangle;

        _spriteBatch.End();
        _graphicsDevice.ScissorRectangle = scissorRectangle;
        _spriteBatch.Begin(
            SpriteSortMode.Deferred,
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            null,
            new RasterizerState { ScissorTestEnable = true });
    }

    public static SpriteBatchScissorScope Begin(SpriteBatch spriteBatch, Rectangle scissorRectangle)
    {
        return new SpriteBatchScissorScope(spriteBatch, scissorRectangle);
    }

    public void Dispose()
    {
        _spriteBatch.End();
        _graphicsDevice.ScissorRectangle = _previousScissorRectangle;
        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
    }
}
