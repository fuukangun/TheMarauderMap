using Microsoft.Xna.Framework;
using TheMarauderMap.Data;

namespace TheMarauderMap.Rendering;

public readonly record struct FootprintSegment(
    Vector2 Start,
    Vector2 End,
    MovementType MovementType,
    float AgeRatio
);
