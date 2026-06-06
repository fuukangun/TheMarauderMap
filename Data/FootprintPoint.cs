using Microsoft.Xna.Framework;

namespace TheMarauderMap.Data;

public readonly record struct FootprintPoint(
    string LocationName,
    Vector2 TilePosition,
    int TimeOfDay,
    MovementType MovementType
);
