namespace TheMarauderMap.UI;

public sealed class MapOverlayStats
{
    public MapOverlayStats(Func<int> getTrackedNpcCount, Func<int> getLastRecordedTime)
    {
        GetTrackedNpcCount = getTrackedNpcCount;
        GetLastRecordedTime = getLastRecordedTime;
    }

    public Func<int> GetTrackedNpcCount { get; }
    public Func<int> GetLastRecordedTime { get; }
}
