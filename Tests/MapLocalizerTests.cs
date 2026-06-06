using TheMarauderMap.Localization;
using Xunit;

namespace TheMarauderMap.Tests;

public sealed class MapLocalizerTests
{
    [Theory]
    [InlineData("zh", MapLanguage.Chinese)]
    [InlineData("zh-Hans", MapLanguage.Chinese)]
    [InlineData("en", MapLanguage.English)]
    [InlineData("ja", MapLanguage.English)]
    public void NormalizeLanguage_UsesChineseOnlyForChineseGameLanguages(string code, MapLanguage expected)
    {
        Assert.Equal(expected, MapLocalizer.NormalizeLanguage(code));
    }

    [Fact]
    public void Text_ReturnsChineseWhenGameLanguageIsChinese()
    {
        Assert.Equal("打开地图按键", MapLocalizer.Text("gmcm.open_map_key.name", MapLanguage.Chinese));
    }

    [Fact]
    public void Text_ReturnsEnglishWhenGameLanguageIsNotChinese()
    {
        Assert.Equal("Open map key", MapLocalizer.Text("gmcm.open_map_key.name", MapLanguage.English));
    }

    [Fact]
    public void GetNpcDisplayName_UsesDisplayNameForChineseAndInternalNameForEnglish()
    {
        Assert.Equal("海莉", MapLocalizer.GetNpcDisplayName("Haley", "海莉", MapLanguage.Chinese));
        Assert.Equal("Haley", MapLocalizer.GetNpcDisplayName("Haley", "海莉", MapLanguage.English));
    }
}
