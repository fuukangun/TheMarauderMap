using Microsoft.Xna.Framework;
using TheMarauderMap.Friendship;
using Xunit;

namespace TheMarauderMap.Tests;

public sealed class FriendshipColorServiceTests
{
    [Theory]
    [InlineData(0, 255, 68, 68)]
    [InlineData(1, 255, 68, 68)]
    [InlineData(2, 255, 136, 68)]
    [InlineData(3, 255, 136, 68)]
    [InlineData(4, 255, 204, 68)]
    [InlineData(6, 255, 204, 68)]
    [InlineData(7, 136, 204, 68)]
    [InlineData(9, 136, 204, 68)]
    [InlineData(10, 68, 204, 136)]
    [InlineData(12, 68, 204, 136)]
    [InlineData(13, 204, 136, 255)]
    [InlineData(14, 204, 136, 255)]
    public void GetColorForHeartLevel_ReturnsExpectedColor(int hearts, byte r, byte g, byte b)
    {
        Color actual = FriendshipColorService.GetColorForHeartLevel(hearts);

        Assert.Equal(new Color(r, g, b), actual);
    }

    [Theory]
    [InlineData(-5, 255, 68, 68)]
    [InlineData(99, 204, 136, 255)]
    public void GetColorForHeartLevel_ClampsOutOfRangeValues(int hearts, byte r, byte g, byte b)
    {
        Color actual = FriendshipColorService.GetColorForHeartLevel(hearts);

        Assert.Equal(new Color(r, g, b), actual);
    }
}
