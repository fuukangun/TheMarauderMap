using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.WorldMaps;

namespace TheMarauderMap.Projection;

public sealed class StardewWorldMapProjectionAdapter : INativeMapProjectionAdapter
{
    public bool TryProject(string locationName, Vector2 tilePosition, out Vector2 mapPosition)
    {
        try
        {
            GameLocation? location = Game1.getLocationFromName(locationName);
            if (location is null)
            {
                mapPosition = Vector2.Zero;
                return false;
            }

            MapAreaPositionWithContext? positionData = WorldMapManager.GetPositionData(location, tilePosition.ToPoint());
            if (!positionData.HasValue)
            {
                mapPosition = Vector2.Zero;
                return false;
            }

            MapAreaPositionWithContext context = positionData.Value;
            Vector2 nativePosition = context.GetMapPixelPosition();
            Rectangle nativeBounds = context.Data.Region.GetMapPixelBounds();
            mapPosition = NativeWorldMapCoordinateMapper.NormalizeToMarauderMap(nativePosition, nativeBounds);
            return true;
        }
        catch
        {
            mapPosition = Vector2.Zero;
            return false;
        }
    }
}
