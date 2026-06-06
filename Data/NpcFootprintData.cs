namespace TheMarauderMap.Data;

public sealed class NpcFootprintData
{
    public NpcFootprintData(string npcName)
    {
        NpcName = npcName;
    }

    public string NpcName { get; }
    public Queue<FootprintPoint> RecentPath { get; } = new();
    public FootprintPoint? LastRecordedPoint { get; set; }
    public bool BreakBeforeNextPoint { get; set; }
}
