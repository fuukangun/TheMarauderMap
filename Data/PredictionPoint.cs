using Microsoft.Xna.Framework;

namespace TheMarauderMap.Data;

public readonly record struct PredictionPoint(
    string LocationName,
    Vector2 TilePosition,
    int TimeOfDay,
    string? Activity
);
