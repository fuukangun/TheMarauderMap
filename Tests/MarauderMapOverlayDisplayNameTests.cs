using TheMarauderMap.Npc;
using Xunit;

namespace TheMarauderMap.Tests;

public sealed class NpcDisplayNameServiceTests
{
    [Fact]
    public void ShouldShowHeart_ReturnsTrueForSpouse()
    {
        string displayName = NpcDisplayNameService.GetMapDisplayName("Haley", "海莉");

        Assert.Equal("海莉", displayName);
        Assert.True(NpcDisplayNameService.ShouldShowHeart("Haley", "Haley"));
    }

    [Fact]
    public void ShouldShowHeart_ReturnsFalseForNonSpouse()
    {
        string displayName = NpcDisplayNameService.GetMapDisplayName("Maru", "玛鲁");

        Assert.Equal("玛鲁", displayName);
        Assert.False(NpcDisplayNameService.ShouldShowHeart("Maru", "Haley"));
    }

    [Fact]
    public void ShouldShowHeart_ReturnsFalseWhenNoSpouse()
    {
        Assert.False(NpcDisplayNameService.ShouldShowHeart("Haley", null));
    }
}
