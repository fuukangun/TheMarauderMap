using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TheMarauderMap.UI;

public sealed class MarauderMapOverlayController
{
    private IMarauderMapOverlay? _overlay;

    public bool IsOpen => _overlay is not null;

    public void Toggle(Func<IMarauderMapOverlay> createOverlay)
    {
        if (_overlay is not null)
        {
            Close();
            return;
        }

        _overlay = createOverlay();
    }

    public void Close()
    {
        _overlay?.Close();
        _overlay = null;
    }

    public void Update(GameTime time)
    {
        _overlay?.Update(time);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _overlay?.Draw(spriteBatch);
    }

    public void ReceiveLeftClick(int x, int y)
    {
        _overlay?.ReceiveLeftClick(x, y);
    }

    public void ReleaseLeftClick(int x, int y)
    {
        _overlay?.ReleaseLeftClick(x, y);
    }

    public void ReceiveRightClick(int x, int y)
    {
        if (_overlay is null)
            return;

        _overlay.ReceiveRightClick(x, y);
        Close();
    }

    public void ReceiveScrollWheel(int direction)
    {
        _overlay?.ReceiveScrollWheel(direction);
    }

    public void ReceiveKeyPress(Keys key)
    {
        if (_overlay is null)
            return;

        _overlay.ReceiveKeyPress(key);
        if (key == Keys.Escape)
            Close();
    }
}
