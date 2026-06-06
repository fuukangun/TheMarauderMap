using TheMarauderMap.Rendering;
using Xunit;

namespace TheMarauderMap.Tests;

public sealed class PredictionRendererTests
{
    [Theory]
    [InlineData(0.25f)]
    [InlineData(0.75f)]
    [InlineData(1.5f)]
    public void GetPathLineWidth_KeepsPredictionPathVisible(float zoom)
    {
        float width = PredictionRenderer.GetPathLineWidth(zoom);

        Assert.InRange(width, 3f, 6f);
    }
}
