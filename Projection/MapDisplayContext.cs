namespace TheMarauderMap.Projection;

public readonly record struct MapDisplayContext(bool IsGingerIsland)
{
    public static MapDisplayContext Mainland { get; } = new(IsGingerIsland: false);
    public static MapDisplayContext GingerIsland { get; } = new(IsGingerIsland: true);

    public static MapDisplayContext FromPlayerLocation(string? locationName)
    {
        return IsGingerIslandLocation(locationName) ? GingerIsland : Mainland;
    }

    public bool AllowsLocation(string? locationName)
    {
        bool isIsland = IsGingerIslandLocation(locationName);
        return IsGingerIsland ? isIsland : !isIsland;
    }

    private static bool IsGingerIslandLocation(string? locationName)
    {
        return !string.IsNullOrWhiteSpace(locationName)
            && locationName.StartsWith("Island", StringComparison.OrdinalIgnoreCase);
    }
}
