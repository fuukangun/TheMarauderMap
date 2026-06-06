using TheMarauderMap.Integrations;
using Xunit;

namespace TheMarauderMap.Tests;

public sealed class GmcmConfigRegistrarTests
{
    [Fact]
    public void GetOptionDefinitions_IncludesUserVisibleMarauderMapSettings()
    {
        IReadOnlyList<string> optionIds = GmcmConfigRegistrar.GetOptionIds();

        Assert.Contains("OpenMapKey", optionIds);
        Assert.Contains("EnableFootprints", optionIds);
        Assert.Contains("EnableFriendshipColors", optionIds);
        Assert.Contains("RecordIntervalMinutes", optionIds);
        Assert.Contains("MaxVisibleFootprintPoints", optionIds);
        Assert.DoesNotContain("ShowDebugOverlay", optionIds);
    }

    [Fact]
    public void GenericModConfigMenuApi_InterfaceIsPublicForSmapiApiMapping()
    {
        Type apiType = typeof(IGenericModConfigMenuApi);

        Assert.True(apiType.IsPublic);
    }
}
