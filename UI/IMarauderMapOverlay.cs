using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TheMarauderMap.UI;

public interface IMarauderMapOverlay
{
    void Update(GameTime time);
    void Draw(SpriteBatch spriteBatch);
    void ReceiveLeftClick(int x, int y);
    void ReleaseLeftClick(int x, int y);
    void ReceiveRightClick(int x, int y);
    void ReceiveScrollWheel(int direction);
    void ReceiveKeyPress(Keys key);
    void Close();
}
