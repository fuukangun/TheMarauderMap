using Microsoft.Xna.Framework;

namespace TheMarauderMap.Projection;

public interface INativeMapProjectionAdapter
{
    bool TryProject(string locationName, Vector2 tilePosition, out Vector2 mapPosition);
}
