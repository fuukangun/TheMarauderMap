using Microsoft.Xna.Framework;
using TheMarauderMap.Data;
using TheMarauderMap.Npc;
using Xunit;

namespace TheMarauderMap.Tests;

public sealed class NpcMapPositionResolverTests
{
    [Fact]
    public void TryGetLatestTrackedPosition_UsesNewestFootprintPoint()
    {
        var footprints = new Dictionary<string, IReadOnlyList<FootprintPoint>>
        {
            ["Abigail"] = new[]
            {
                new FootprintPoint("Town", new Vector2(10, 10), 900, MovementType.Normal),
                new FootprintPoint("Town", new Vector2(25, 35), 910, MovementType.Normal)
            }
        };

        bool success = NpcMapPositionResolver.TryGetLatestTrackedPosition("Abigail", footprints, out FootprintPoint point);

        Assert.True(success);
        Assert.Equal(new Vector2(25, 35), point.TilePosition);
        Assert.Equal(910, point.TimeOfDay);
    }
}
