using TheMarauderMap.Localization;
using Xunit;

namespace TheMarauderMap.Tests;

public sealed class MapLanguageServiceTests
{
    [Theory]
    [InlineData("zh", true)]
    [InlineData("zh-Hans", true)]
    [InlineData("en", false)]
    [InlineData("ja", false)]
    public void IsChinese_UsesChineseOnlyForChineseGameLanguages(string code, bool expected)
    {
        Assert.Equal(expected, MapLanguageService.IsChinese(code));
    }

    [Fact]
    public void I18nFiles_DefineAllGmcmTranslationKeys()
    {
        IReadOnlyList<string> requiredKeys = new[]
        {
            "gmcm.open_map_key.name",
            "gmcm.open_map_key.tooltip",
            "gmcm.enable_footprints.name",
            "gmcm.enable_footprints.tooltip",
            "gmcm.enable_friendship_colors.name",
            "gmcm.enable_friendship_colors.tooltip",
            "gmcm.footprint_interval.name",
            "gmcm.footprint_interval.tooltip",
            "gmcm.visible_footprints.name",
            "gmcm.visible_footprints.tooltip"
        };

        Assert.Contains("\"Open map key\"", File.ReadAllText(GetRepoPath("i18n/default.json")));
        Assert.Contains("\"打开地图按键\"", File.ReadAllText(GetRepoPath("i18n/zh.json")));

        foreach (string key in requiredKeys)
        {
            Assert.Contains($"\"{key}\"", File.ReadAllText(GetRepoPath("i18n/default.json")));
            Assert.Contains($"\"{key}\"", File.ReadAllText(GetRepoPath("i18n/zh.json")));
        }
    }

    [Fact]
    public void GetNpcDisplayName_UsesDisplayNameForChineseAndInternalNameForEnglish()
    {
        Assert.Equal("海莉", MapLanguageService.GetNpcDisplayName("Haley", "海莉", isChinese: true));
        Assert.Equal("Haley", MapLanguageService.GetNpcDisplayName("Haley", "海莉", isChinese: false));
    }

    private static string GetRepoPath(string relativePath)
    {
        return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../", relativePath));
    }
}
