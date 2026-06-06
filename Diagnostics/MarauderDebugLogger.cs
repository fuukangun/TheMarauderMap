using Microsoft.Xna.Framework;
using StardewModdingAPI;
using TheMarauderMap.Config;

namespace TheMarauderMap.Diagnostics;

public sealed class MarauderDebugLogger
{
    private readonly IMonitor _monitor;
    private readonly MarauderMapConfig _config;
    private readonly HashSet<string> _reportedProjectionFailures = new(StringComparer.OrdinalIgnoreCase);

    public MarauderDebugLogger(IMonitor monitor, MarauderMapConfig config)
    {
        _monitor = monitor;
        _config = config;
    }

    public void MapOpened(int trackedNpcCount)
    {
        if (!_config.EnableDebugLogging)
            return;

        _monitor.Log($"[UI] Marauder map opened. Tracked NPCs: {trackedNpcCount}.", LogLevel.Debug);
    }

    public void FootprintRecorded(string npcName, string locationName, Vector2 tile, int timeOfDay)
    {
        if (!_config.EnableDebugLogging || !_config.LogFootprintRecording)
            return;

        _monitor.Log($"[Footprint] {timeOfDay} {npcName} @ {locationName} {tile}.", LogLevel.Trace);
    }

    public void ProjectionFailed(string source, string npcName, string locationName)
    {
        if (!_config.EnableDebugLogging || !_config.LogProjectionFailures)
            return;

        string key = $"{source}:{npcName}:{locationName}";
        if (!_reportedProjectionFailures.Add(key))
            return;

        _monitor.Log($"[Projection] Failed to project {npcName} in {locationName} from {source}.", LogLevel.Warn);
    }

    public void DayReset(string reason)
    {
        _reportedProjectionFailures.Clear();

        if (!_config.EnableDebugLogging)
            return;

        _monitor.Log($"[Lifecycle] Cleared footprint and projection debug state: {reason}.", LogLevel.Debug);
    }
}
