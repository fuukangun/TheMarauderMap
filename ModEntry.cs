using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using TheMarauderMap.Config;
using TheMarauderMap.Diagnostics;
using TheMarauderMap.Integrations;
using TheMarauderMap.Localization;
using TheMarauderMap.Npc;
using TheMarauderMap.Player;
using TheMarauderMap.Projection;
using TheMarauderMap.Tracker;
using TheMarauderMap.UI;

namespace TheMarauderMap;

public sealed class ModEntry : Mod
{
    private MarauderMapConfig _config = new();
    private NpcFootprintTracker _tracker = null!;
    private MapProjectionService _projection = null!;
    private MarauderDebugLogger _debugLogger = null!;
    private MarauderMapOverlayController _overlayController = null!;
    private readonly Random _random = new();
    private int _lastRecordedTime = -1;

    public override void Entry(IModHelper helper)
    {
        _config = helper.ReadConfig<MarauderMapConfig>();
        NormalizeConfig();

        _tracker = new NpcFootprintTracker(_config.MaxStoredFootprintPoints);
        _projection = new MapProjectionService(new StardewWorldMapProjectionAdapter());
        _debugLogger = new MarauderDebugLogger(Monitor, _config);
        _overlayController = new MarauderMapOverlayController();

        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.Input.ButtonPressed += OnButtonPressed;
        helper.Events.Input.ButtonReleased += OnButtonReleased;
        helper.Events.Input.MouseWheelScrolled += OnMouseWheelScrolled;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        helper.Events.Display.RenderedHud += OnRenderedHud;
        helper.Events.GameLoop.TimeChanged += OnTimeChanged;
        helper.Events.GameLoop.DayEnding += OnDayEnding;
        helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;

        Monitor.Log("The Marauder's Map loaded. Press H to open the map.", LogLevel.Info);
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        GmcmConfigRegistrar.Register(Helper, ModManifest, () => _config, config => _config = config, SaveConfig, GetLanguage);
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        _tracker.ClearAll();
        _overlayController.Close();
        _debugLogger.DayReset("save loaded");
        _lastRecordedTime = -1;
    }

    private void OnDayEnding(object? sender, DayEndingEventArgs e)
    {
        _tracker.ClearAll();
        _overlayController.Close();
        _debugLogger.DayReset("day ending");
        _lastRecordedTime = -1;
    }

    private void OnTimeChanged(object? sender, TimeChangedEventArgs e)
    {
        if (!Context.IsWorldReady)
            return;

        if (!_config.EnableFootprints)
            return;

        if (!ShouldRecordAtTime(e.NewTime))
            return;

        foreach (NPC npc in Utility.getAllCharacters())
        {
            if (!NpcFilter.ShouldShowOnMap(npc))
                continue;

            _tracker.RecordPoint(npc.Name, npc.currentLocation.Name, npc.Tile, e.NewTime);
            _debugLogger.FootprintRecorded(npc.Name, npc.currentLocation.Name, npc.Tile, e.NewTime);
        }

        _lastRecordedTime = e.NewTime;
    }

    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (!Context.IsWorldReady)
            return;

        if (_overlayController.IsOpen)
        {
            if (_config.OpenMapKey.JustPressed())
            {
                _overlayController.Close();
                Helper.Input.Suppress(e.Button);
                return;
            }

            if (e.Button == SButton.MouseLeft || e.Button.IsActionButton())
            {
                Helper.Input.Suppress(e.Button);
                return;
            }

            if (e.Button == SButton.MouseRight)
            {
                Point cursor = Game1.getMousePosition();
                _overlayController.ReceiveRightClick(cursor.X, cursor.Y);
                Helper.Input.Suppress(e.Button);
                return;
            }

            if (e.Button == SButton.Escape)
            {
                _overlayController.ReceiveKeyPress(Microsoft.Xna.Framework.Input.Keys.Escape);
                Helper.Input.Suppress(e.Button);
            }

            return;
        }

        if (!_config.OpenMapKey.JustPressed())
            return;

        if (Game1.activeClickableMenu is not null)
            return;

        int trackedNpcCount = _tracker.GetTrackedNpcCount();
        _debugLogger.MapOpened(trackedNpcCount);

        DeductMapStaminaCost();
        _overlayController.Toggle(CreateOverlay);
        Helper.Input.Suppress(e.Button);
    }

    private void OnButtonReleased(object? sender, ButtonReleasedEventArgs e)
    {
        if (!_overlayController.IsOpen)
            return;

        if (e.Button == SButton.MouseLeft || e.Button.IsActionButton())
        {
            Helper.Input.Suppress(e.Button);
        }
    }

    private void OnMouseWheelScrolled(object? sender, MouseWheelScrolledEventArgs e)
    {
        if (!_overlayController.IsOpen)
            return;

        Helper.Input.SuppressScrollWheel();
        _overlayController.ReceiveScrollWheel(e.Delta);
    }

    private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (!_overlayController.IsOpen)
            return;

        _overlayController.Update(Game1.currentGameTime);
    }

    private void OnRenderedHud(object? sender, RenderedHudEventArgs e)
    {
        if (!_overlayController.IsOpen)
            return;

        _overlayController.Draw(e.SpriteBatch);
    }

    private MarauderMapOverlay CreateOverlay()
    {
        return new MarauderMapOverlay(
            _projection,
            _debugLogger,
            () => _tracker.GetAllFootprints(),
            () => _config.MaxVisibleFootprintPoints,
            () => _config.EnableFootprints,
            () => _config.EnableFriendshipColors,
            () => _config.ShowDebugOverlay,
            GetLanguage,
            new MapOverlayStats(
                () => _tracker.GetTrackedNpcCount(),
                () => _lastRecordedTime
            ),
            Helper.DirectoryPath
        );
    }

    private static MapLanguage GetLanguage()
    {
        return MapLocalizer.NormalizeLanguage(StardewValley.LocalizedContentManager.CurrentLanguageCode.ToString());
    }

    private bool ShouldRecordAtTime(int timeOfDay)
    {
        if (_lastRecordedTime == timeOfDay)
            return false;

        int minutes = TimeOfDayToMinutes(timeOfDay);
        return minutes % _config.RecordIntervalMinutes == 0;
    }

    private void NormalizeConfig()
    {
        if (_config.RecordIntervalMinutes != 10 && _config.RecordIntervalMinutes != 20 && _config.RecordIntervalMinutes != 30)
            _config.RecordIntervalMinutes = 10;

        _config.MaxStoredFootprintPoints = Math.Clamp(_config.MaxStoredFootprintPoints, 5, 200);
        _config.MaxVisibleFootprintPoints = Math.Clamp(_config.MaxVisibleFootprintPoints, 2, _config.MaxStoredFootprintPoints);

        Helper.WriteConfig(_config);
    }

    private void SaveConfig()
    {
        NormalizeConfig();
        Helper.WriteConfig(_config);
    }

    private void DeductMapStaminaCost()
    {
        int cost = MagicMapStaminaCostService.RollCost(_random);
        Game1.player.Stamina = MagicMapStaminaCostService.ApplyCost(Game1.player.Stamina, cost);
    }

    private static int TimeOfDayToMinutes(int timeOfDay)
    {
        int hours = timeOfDay / 100;
        int minutes = timeOfDay % 100;
        return hours * 60 + minutes;
    }
}
