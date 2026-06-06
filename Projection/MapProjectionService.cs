using Microsoft.Xna.Framework;

namespace TheMarauderMap.Projection;

public sealed class MapProjectionService
{
    public const int MapWidth = 1600;
    public const int MapHeight = 1000;
    private readonly INativeMapProjectionAdapter? _nativeProjection;

    public MapProjectionService(INativeMapProjectionAdapter? nativeProjection = null)
    {
        _nativeProjection = nativeProjection;
    }

    private readonly Dictionary<string, ProjectionRegion> _outdoorRegions = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Farm"] = new(new Rectangle(70, 360, 390, 360), new Vector2(0, 0), new Vector2(100, 100)),
        ["BusStop"] = new(new Rectangle(480, 430, 160, 130), new Vector2(0, 0), new Vector2(40, 40)),
        ["Town"] = new(new Rectangle(650, 390, 500, 360), new Vector2(0, 0), new Vector2(120, 120)),
        ["Mountain"] = new(new Rectangle(680, 130, 460, 240), new Vector2(0, 0), new Vector2(120, 80)),
        ["Forest"] = new(new Rectangle(470, 720, 420, 220), new Vector2(0, 0), new Vector2(120, 80)),
        ["Beach"] = new(new Rectangle(900, 760, 360, 180), new Vector2(0, 0), new Vector2(120, 60)),
        ["Railroad"] = new(new Rectangle(760, 25, 350, 90), new Vector2(0, 0), new Vector2(120, 40)),
        ["Desert"] = new(new Rectangle(1250, 620, 280, 240), new Vector2(0, 0), new Vector2(100, 100))
    };

    private readonly Dictionary<string, Vector2> _anchors = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Saloon"] = new(955, 560),
        ["ScienceHouse"] = new(770, 310),
        ["ManorHouse"] = new(830, 570),
        ["SeedShop"] = new(900, 520),
        ["Blacksmith"] = new(1080, 500),
        ["Hospital"] = new(960, 455),
        ["Carpenter"] = new(850, 240),
        ["AnimalShop"] = new(635, 780),
        ["ArchaeologyHouse"] = new(1120, 555),
        ["FishShop"] = new(1040, 840),
        ["ElliottHouse"] = new(1150, 810),
        ["Trailer"] = new(850, 455),
        ["HarveyRoom"] = new(960, 455),
        ["CommunityCenter"] = new(800, 420),
        ["JojaMart"] = new(1135, 445),
        ["WizardHouse"] = new(520, 850),
        ["AdventureGuild"] = new(1080, 210),
        ["Mine"] = new(1010, 250),
        ["SamHouse"] = new(790, 610),
        ["HaleyHouse"] = new(780, 650),
        ["JoshHouse"] = new(780, 560),
        ["LeahHouse"] = new(610, 835),
        ["SebastianRoom"] = new(850, 240),
        ["Tent"] = new(930, 230),
        ["BathHouse_Entry"] = new(880, 105),
        ["BathHouse_MensLocker"] = new(880, 105),
        ["BathHouse_WomensLocker"] = new(880, 105),
        ["BathHouse_Pool"] = new(880, 105),
        ["Club"] = new(1370, 700),
        ["SandyHouse"] = new(1370, 700),
        ["IslandSouth"] = new(1400, 900),
        ["IslandNorth"] = new(1400, 780),
        ["IslandWest"] = new(1300, 850),
        ["IslandEast"] = new(1500, 850)
    };

    public bool TryProject(string locationName, Vector2 tilePosition, out Vector2 mapPosition)
    {
        if (_nativeProjection is not null && _nativeProjection.TryProject(locationName, tilePosition, out mapPosition))
            return true;

        return TryProjectFallback(locationName, tilePosition, out mapPosition);
    }

    private bool TryProjectFallback(string locationName, Vector2 tilePosition, out Vector2 mapPosition)
    {
        if (_outdoorRegions.TryGetValue(locationName, out ProjectionRegion region))
        {
            mapPosition = region.Project(tilePosition);
            return true;
        }

        return TryGetLocationAnchor(locationName, out mapPosition);
    }

    public bool TryGetLocationAnchor(string locationName, out Vector2 mapPosition)
    {
        if (_anchors.TryGetValue(locationName, out mapPosition))
            return true;

        mapPosition = Vector2.Zero;
        return false;
    }

    private readonly record struct ProjectionRegion(Rectangle MapRect, Vector2 TileMin, Vector2 TileMax)
    {
        public Vector2 Project(Vector2 tile)
        {
            float normalizedX = Normalize(tile.X, TileMin.X, TileMax.X);
            float normalizedY = Normalize(tile.Y, TileMin.Y, TileMax.Y);

            return new Vector2(
                MapRect.X + normalizedX * MapRect.Width,
                MapRect.Y + normalizedY * MapRect.Height
            );
        }

        private static float Normalize(float value, float min, float max)
        {
            if (Math.Abs(max - min) < 0.001f)
                return 0f;

            return MathHelper.Clamp((value - min) / (max - min), 0f, 1f);
        }
    }

}
