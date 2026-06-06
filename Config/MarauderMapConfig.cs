using StardewModdingAPI.Utilities;

namespace TheMarauderMap.Config;

public sealed class MarauderMapConfig
{
    public bool EnableFootprints { get; set; } = true;
    public bool EnableFriendshipColors { get; set; } = true;
    public bool EnableDebugLogging { get; set; } = false;
    public bool LogFootprintRecording { get; set; } = false;
    public bool LogProjectionFailures { get; set; } = true;
    public bool ShowDebugOverlay { get; set; } = false;

    public int RecordIntervalMinutes { get; set; } = 10;
    public int MaxStoredFootprintPoints { get; set; } = 40;
    public int MaxVisibleFootprintPoints { get; set; } = 12;

    public KeybindList OpenMapKey { get; set; } = KeybindList.Parse("H");
}
