namespace TheMarauderMap.Rendering;

public static class MapTextureAssetSelector
{
    public static IReadOnlyList<string> GetLegacyFallbackAssetNames()
    {
        return new[]
        {
            "LooseSprites/map",
            "LooseSprites/map_spring",
            "LooseSprites/map_summer",
            "LooseSprites/map_fall",
            "LooseSprites/map_winter"
        };
    }
}
