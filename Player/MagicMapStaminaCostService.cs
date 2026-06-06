namespace TheMarauderMap.Player;

public static class MagicMapStaminaCostService
{
    public static int RollCost(Random random)
    {
        ArgumentNullException.ThrowIfNull(random);

        return random.Next(4, 9);
    }

    public static float ApplyCost(float stamina, int cost)
    {
        return Math.Max(0f, stamina - cost);
    }
}
