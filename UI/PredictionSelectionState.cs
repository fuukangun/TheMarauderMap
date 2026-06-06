namespace TheMarauderMap.UI;

public sealed class PredictionSelectionState
{
    public string? SelectedNpcName { get; private set; }

    public void Select(string npcName)
    {
        SelectedNpcName = string.Equals(SelectedNpcName, npcName, StringComparison.OrdinalIgnoreCase)
            ? null
            : npcName;
    }

    public void Clear()
    {
        SelectedNpcName = null;
    }
}
