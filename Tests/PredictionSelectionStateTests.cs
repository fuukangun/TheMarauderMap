using TheMarauderMap.UI;
using Xunit;

namespace TheMarauderMap.Tests;

public sealed class PredictionSelectionStateTests
{
    [Fact]
    public void Select_TogglesPredictionTargetForSameNpc()
    {
        var state = new PredictionSelectionState();

        state.Select("Abigail");
        state.Select("Abigail");

        Assert.Null(state.SelectedNpcName);
    }

    [Fact]
    public void Select_ReplacesPredictionTargetForDifferentNpc()
    {
        var state = new PredictionSelectionState();

        state.Select("Abigail");
        state.Select("Sebastian");

        Assert.Equal("Sebastian", state.SelectedNpcName);
    }
}
