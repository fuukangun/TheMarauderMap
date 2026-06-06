using TheMarauderMap.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Xunit;

namespace TheMarauderMap.Tests;

public sealed class MarauderMapOverlayControllerTests
{
    [Fact]
    public void Toggle_OpensWithoutRequiringActiveClickableMenu()
    {
        var controller = new MarauderMapOverlayController();

        controller.Toggle(() => new FakeOverlay());

        Assert.True(controller.IsOpen);
    }

    [Fact]
    public void Toggle_WhenOpen_ClosesOverlay()
    {
        var controller = new MarauderMapOverlayController();

        controller.Toggle(() => new FakeOverlay());
        controller.Toggle(() => throw new InvalidOperationException("Should close existing overlay."));

        Assert.False(controller.IsOpen);
    }

    [Fact]
    public void MouseInput_ForwardsPressAndReleaseSeparately()
    {
        var controller = new MarauderMapOverlayController();
        var overlay = new FakeOverlay();

        controller.Toggle(() => overlay);
        controller.ReceiveLeftClick(10, 20);
        controller.ReleaseLeftClick(30, 40);

        Assert.Equal((10, 20), overlay.LeftPressed);
        Assert.Equal((30, 40), overlay.LeftReleased);
    }

    private sealed class FakeOverlay : IMarauderMapOverlay
    {
        public (int X, int Y)? LeftPressed { get; private set; }
        public (int X, int Y)? LeftReleased { get; private set; }

        public void Update(GameTime time) { }
        public void Draw(SpriteBatch spriteBatch) { }
        public void ReceiveLeftClick(int x, int y) => LeftPressed = (x, y);
        public void ReleaseLeftClick(int x, int y) => LeftReleased = (x, y);
        public void ReceiveRightClick(int x, int y) { }
        public void ReceiveScrollWheel(int direction) { }
        public void ReceiveKeyPress(Keys key) { }
        public void Close() { }
    }
}
