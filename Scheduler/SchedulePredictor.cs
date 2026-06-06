using Microsoft.Xna.Framework;
using StardewValley;
using TheMarauderMap.Data;

namespace TheMarauderMap.Scheduler;

public sealed class SchedulePredictor
{
    public IReadOnlyList<PredictionPoint> GetPredictedPath(NPC npc, int currentTime)
    {
        try
        {
            var schedule = npc.Schedule;
            if (schedule is null || schedule.Count == 0)
                return Array.Empty<PredictionPoint>();

            return SchedulePredictionBuilder.BuildUpcomingPredictions(
                schedule.Select(entry => new SchedulePredictionEntry(
                    entry.Key,
                    entry.Value.targetLocationName,
                    new Vector2(entry.Value.targetTile.X, entry.Value.targetTile.Y),
                    null
                )),
                currentTime,
                maxPredictions: 4
            );
        }
        catch
        {
            return Array.Empty<PredictionPoint>();
        }
    }
}
