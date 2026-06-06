using TheMarauderMap.Player;
using Xunit;

namespace TheMarauderMap.Tests;

public sealed class MagicMapStaminaCostServiceTests
{
    [Fact]
    public void RollCost_ReturnsCostInExpectedRange()
    {
        Random random = new(42);

        for (int i = 0; i < 100; i++)
        {
            int cost = MagicMapStaminaCostService.RollCost(random);

            Assert.InRange(cost, 4, 8);
        }
    }

    [Fact]
    public void ApplyCost_SubtractsCost()
    {
        float stamina = MagicMapStaminaCostService.ApplyCost(20f, 6);

        Assert.Equal(14f, stamina);
    }

    [Fact]
    public void ApplyCost_DoesNotGoBelowZero()
    {
        float stamina = MagicMapStaminaCostService.ApplyCost(3f, 8);

        Assert.Equal(0f, stamina);
    }
}
