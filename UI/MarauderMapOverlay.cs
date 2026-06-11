using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using TheMarauderMap.Data;
using TheMarauderMap.Diagnostics;
using TheMarauderMap.Friendship;
using TheMarauderMap.Localization;
using TheMarauderMap.Npc;
using TheMarauderMap.Projection;
using TheMarauderMap.Rendering;

namespace TheMarauderMap.UI;

public sealed class MarauderMapOverlay : IClickableMenu, IMarauderMapOverlay
{
    private readonly MapProjectionService _projection;
    private readonly MarauderDebugLogger _debugLogger;
    private readonly MapBackgroundRenderer _backgroundRenderer;
    private readonly MapContentRenderer _contentRenderer;
    private readonly FootprintRenderer _footprintRenderer;
    private readonly NameRenderer _nameRenderer;
    private readonly Func<IReadOnlyDictionary<string, IReadOnlyList<FootprintPoint>>> _getFootprints;
    private readonly Func<int> _getMaxVisibleFootprints;
    private readonly Func<bool> _areFootprintsEnabled;
    private readonly Func<bool> _areFriendshipColorsEnabled;
    private readonly Func<bool> _shouldShowDebugOverlay;
    private readonly Func<bool> _isChineseLanguage;
    private readonly MapOverlayStats _stats;

    private readonly PredictionSelectionState _predictionSelection = new();
    private MapContentViewportState? _contentViewport;
    private bool _dragging;
    private bool _dragExceededClickThreshold;
    private float _dragDistanceSquared;
    private int _pendingScrollDirection;
    private int? _lastScrollWheelValue;
    private int _lastScrollWheelDelta;
    private Point _lastMouse;
    private bool _leftMouseWasDown;
    private double _longPressTimer;
    private Rectangle? _lastNpcCoordinateBounds;
    private const double LongPressThresholdMs = 500;
    private const float DragClickThresholdPixels = 8f;
    private MapDisplayContext DisplayContext => MapDisplayContext.FromPlayerLocation(Game1.player?.currentLocation?.Name);

    public MarauderMapOverlay(
        MapProjectionService projection,
        MarauderDebugLogger debugLogger,
        Func<IReadOnlyDictionary<string, IReadOnlyList<FootprintPoint>>> getFootprints,
        Func<int> getMaxVisibleFootprints,
        Func<bool> areFootprintsEnabled,
        Func<bool> areFriendshipColorsEnabled,
        Func<bool> shouldShowDebugOverlay,
        Func<bool> isChineseLanguage,
        MapOverlayStats stats,
        string modDirectory)
        : base(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height, true)
    {
        _projection = projection;
        _debugLogger = debugLogger;
        _backgroundRenderer = new MapBackgroundRenderer();
        _contentRenderer = new MapContentRenderer();
        _footprintRenderer = new FootprintRenderer(projection, modDirectory);
        _nameRenderer = new NameRenderer(modDirectory);
        _getFootprints = getFootprints;
        _getMaxVisibleFootprints = getMaxVisibleFootprints;
        _areFootprintsEnabled = areFootprintsEnabled;
        _areFriendshipColorsEnabled = areFriendshipColorsEnabled;
        _shouldShowDebugOverlay = shouldShowDebugOverlay;
        _isChineseLanguage = isChineseLanguage;
        _stats = stats;
    }

    public override void update(GameTime time)
    {
        base.update(time);

        Point mousePoint = Game1.getMousePosition();
        PollScrollWheel();
        PollLeftMouse(mousePoint);

        if (_dragging && _contentViewport is not null)
        {
            Vector2 screenDelta = new(mousePoint.X - _lastMouse.X, mousePoint.Y - _lastMouse.Y);
            if (screenDelta != Vector2.Zero)
            {
                _contentViewport.PanScreenDelta(screenDelta);
                _dragDistanceSquared += screenDelta.LengthSquared();
                _dragExceededClickThreshold = _dragDistanceSquared > DragClickThresholdPixels * DragClickThresholdPixels;
                if (_dragExceededClickThreshold)
                    _longPressTimer = 0;
                _lastMouse = mousePoint;
            }
        }

        if (_longPressTimer > 0)
        {
            _longPressTimer += time.ElapsedGameTime.TotalMilliseconds;
            if (_longPressTimer >= LongPressThresholdMs)
            {
                _longPressTimer = 0;
                TogglePredictionForHoveredNpc();
            }
        }

    }

    private void PollLeftMouse(Point mousePoint)
    {
        bool leftMouseDown = Mouse.GetState().LeftButton == ButtonState.Pressed;
        if (leftMouseDown && !_leftMouseWasDown)
            BeginLeftPress(mousePoint);
        else if (!leftMouseDown && _leftMouseWasDown)
            EndLeftPress();

        _leftMouseWasDown = leftMouseDown;
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        BeginLeftPress(new Point(x, y));
    }

    public override void releaseLeftClick(int x, int y)
    {
        EndLeftPress();
    }

    private void BeginLeftPress(Point mousePoint)
    {
        _dragging = _contentViewport?.IsZoomed == true;
        _dragExceededClickThreshold = false;
        _dragDistanceSquared = 0f;
        _lastMouse = mousePoint;
        _longPressTimer = 1;
    }

    private void EndLeftPress()
    {
        bool wasDrag = _dragExceededClickThreshold;
        _dragging = false;

        if (!wasDrag && _longPressTimer > 0 && _longPressTimer < LongPressThresholdMs)
            TrySelectNpcAtCursor();

        _longPressTimer = 0;
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
        _predictionSelection.Clear();
    }

    protected override void cleanupBeforeExit()
    {
        _backgroundRenderer.Dispose();
        _contentRenderer.Dispose();
        _footprintRenderer.Dispose();
        base.cleanupBeforeExit();
    }

    public void Close()
    {
        cleanupBeforeExit();
    }

    void IMarauderMapOverlay.Update(GameTime time) => update(time);
    void IMarauderMapOverlay.Draw(SpriteBatch spriteBatch) => draw(spriteBatch);
    void IMarauderMapOverlay.ReceiveLeftClick(int x, int y) => receiveLeftClick(x, y);
    void IMarauderMapOverlay.ReleaseLeftClick(int x, int y) => releaseLeftClick(x, y);
    void IMarauderMapOverlay.ReceiveRightClick(int x, int y) => receiveRightClick(x, y);
    void IMarauderMapOverlay.ReceiveScrollWheel(int direction) => ReceiveExternalScrollWheel(direction, Mouse.GetState().ScrollWheelValue);
    void IMarauderMapOverlay.ReceiveKeyPress(Keys key) => receiveKeyPress(key);

    public override void receiveScrollWheelAction(int direction)
    {
        _lastScrollWheelValue = Mouse.GetState().ScrollWheelValue;
        _lastScrollWheelDelta = direction;
        ApplyScrollZoom(direction);
    }

    public void ReceiveExternalScrollWheel(int direction, int currentScrollWheelValue)
    {
        _lastScrollWheelValue = currentScrollWheelValue;
        _lastScrollWheelDelta = direction;
        ApplyScrollZoom(direction);
    }

    public override void receiveKeyPress(Keys key)
    {
    }

    public override void draw(SpriteBatch b)
    {
        _backgroundRenderer.Draw(b, Vector2.Zero, 1f);
        UpdateContentViewport();

        bool useZoomedContent = _contentViewport?.IsZoomed == true;
        MapContentTransform? contentTransform = CreateContentTransform();

        if (useZoomedContent && contentTransform.HasValue)
        {
            _contentRenderer.Draw(
                b,
                contentTransform.Value.ContentBounds,
                contentTransform.Value.PanOffset,
                contentTransform.Value.Zoom);

            using (SpriteBatchScissorScope.Begin(b, contentTransform.Value.ContentBounds))
                DrawMapOverlays(b, contentTransform.Value);
        }
        else
        {
            DrawMapOverlays(b, CreateMapScreenTransform());
        }

        DrawDebugOverlay(b);
        drawMouse(b);
    }

    private void DrawMapOverlays(SpriteBatch b, MapScreenTransform transform)
    {
        if (_areFootprintsEnabled())
            _footprintRenderer.Draw(b, _getFootprints(), _getMaxVisibleFootprints(), _predictionSelection.SelectedNpcName, DisplayContext, transform);

        IReadOnlyDictionary<string, IReadOnlyList<FootprintPoint>> footprints = _getFootprints();
        Rectangle? npcCoordinateBounds = null;

        foreach (NPC npc in Utility.getAllCharacters())
        {
            if (!NpcFilter.ShouldShowOnMap(npc))
                continue;

            if (!DisplayContext.AllowsLocation(npc.currentLocation?.Name))
                continue;

            if (!TryGetNpcMapPosition(npc, footprints, out Vector2 mapPosition))
            {
                _debugLogger.ProjectionFailed("NameRenderer", npc.Name, npc.currentLocation?.Name ?? "unknown");
                continue;
            }

            IncludeNpcCoordinate(ref npcCoordinateBounds, mapPosition);
            Vector2 screenPosition = transform.MapToScreen(mapPosition);
            Color color = Color.Wheat;
            if (_areFriendshipColorsEnabled())
            {
                int hearts = Game1.player.getFriendshipHeartLevelForNPC(npc.Name);
                color = FriendshipColorService.GetColorForHeartLevel(hearts);
            }

            bool isSelected = string.Equals(npc.Name, _predictionSelection.SelectedNpcName, StringComparison.OrdinalIgnoreCase);
            _nameRenderer.DrawName(b, GetMapDisplayName(npc, _isChineseLanguage()), screenPosition, color, 1f, isSelected, NpcDisplayNameService.ShouldShowHeart(npc.Name, Game1.player.spouse));
        }

        _lastNpcCoordinateBounds = npcCoordinateBounds;
    }

    private void DrawMapOverlays(SpriteBatch b, MapContentTransform transform)
    {
        if (_areFootprintsEnabled())
            _footprintRenderer.Draw(b, _getFootprints(), _getMaxVisibleFootprints(), _predictionSelection.SelectedNpcName, DisplayContext, transform);

        IReadOnlyDictionary<string, IReadOnlyList<FootprintPoint>> footprints = _getFootprints();
        Rectangle? npcCoordinateBounds = null;

        foreach (NPC npc in Utility.getAllCharacters())
        {
            if (!NpcFilter.ShouldShowOnMap(npc))
                continue;

            if (!DisplayContext.AllowsLocation(npc.currentLocation?.Name))
                continue;

            if (!TryGetNpcMapPosition(npc, footprints, out Vector2 mapPosition))
            {
                _debugLogger.ProjectionFailed("NameRenderer", npc.Name, npc.currentLocation?.Name ?? "unknown");
                continue;
            }

            IncludeNpcCoordinate(ref npcCoordinateBounds, mapPosition);
            Vector2 screenPosition = transform.MapToScreen(mapPosition);
            Color color = Color.Wheat;
            if (_areFriendshipColorsEnabled())
            {
                int hearts = Game1.player.getFriendshipHeartLevelForNPC(npc.Name);
                color = FriendshipColorService.GetColorForHeartLevel(hearts);
            }

            bool isSelected = string.Equals(npc.Name, _predictionSelection.SelectedNpcName, StringComparison.OrdinalIgnoreCase);
            _nameRenderer.DrawName(b, GetMapDisplayName(npc, _isChineseLanguage()), screenPosition, color, transform.Zoom, isSelected, NpcDisplayNameService.ShouldShowHeart(npc.Name, Game1.player.spouse));
        }

        _lastNpcCoordinateBounds = npcCoordinateBounds;
    }

    private static void IncludeNpcCoordinate(ref Rectangle? bounds, Vector2 mapPosition)
    {
        var pointBounds = new Rectangle(
            (int)MathF.Floor(mapPosition.X),
            (int)MathF.Floor(mapPosition.Y),
            1,
            1);

        bounds = bounds.HasValue ? Rectangle.Union(bounds.Value, pointBounds) : pointBounds;
    }

    public static string GetMapDisplayName(NPC npc)
    {
        return GetMapDisplayName(npc, isChinese: true);
    }

    public static string GetMapDisplayName(NPC npc, bool isChinese)
    {
        return NpcDisplayNameService.GetMapDisplayName(npc.Name, npc.displayName, isChinese);
    }

    private void TrySelectNpcAtCursor()
    {
        Point mousePoint = Game1.getMousePosition();
        IReadOnlyDictionary<string, IReadOnlyList<FootprintPoint>> footprints = _getFootprints();
        Vector2 mouseScreen = mousePoint.ToVector2();
        Func<Vector2, Vector2> mapToScreen = CreateMapToScreenFunc();

        float closestDistance = float.MaxValue;
        NPC? closestNpc = null;
        Vector2 closestScreen = Vector2.Zero;

        foreach (NPC npc in Utility.getAllCharacters())
        {
            if (!NpcFilter.ShouldShowOnMap(npc))
                continue;

            if (!DisplayContext.AllowsLocation(npc.currentLocation?.Name))
                continue;

            if (!TryGetNpcMapPosition(npc, footprints, out Vector2 mapPosition))
                continue;

            Vector2 npcScreen = mapToScreen(mapPosition);
            float distance = Vector2.Distance(mouseScreen, npcScreen);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestNpc = npc;
                closestScreen = npcScreen;
            }
        }

        if (closestNpc is not null && MapHitTest.IsWithinScreenRadius(mouseScreen, closestScreen, 60f))
            _predictionSelection.Select(closestNpc.Name);
        else
            _predictionSelection.Clear();
    }

    private void TogglePredictionForHoveredNpc()
    {
        Point mousePoint = Game1.getMousePosition();
        IReadOnlyDictionary<string, IReadOnlyList<FootprintPoint>> footprints = _getFootprints();
        Vector2 mouseScreen = mousePoint.ToVector2();
        Func<Vector2, Vector2> mapToScreen = CreateMapToScreenFunc();

        float closestDistance = float.MaxValue;
        NPC? closestNpc = null;
        Vector2 closestScreen = Vector2.Zero;

        foreach (NPC npc in Utility.getAllCharacters())
        {
            if (!NpcFilter.ShouldShowOnMap(npc))
                continue;

            if (!DisplayContext.AllowsLocation(npc.currentLocation?.Name))
                continue;

            if (!TryGetNpcMapPosition(npc, footprints, out Vector2 mapPosition))
                continue;

            Vector2 npcScreen = mapToScreen(mapPosition);
            float distance = Vector2.Distance(mouseScreen, npcScreen);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestNpc = npc;
                closestScreen = npcScreen;
            }
        }

        if (closestNpc is not null && MapHitTest.IsWithinScreenRadius(mouseScreen, closestScreen, 60f))
            _predictionSelection.Select(closestNpc.Name);
    }

    private bool TryGetNpcMapPosition(NPC npc, IReadOnlyDictionary<string, IReadOnlyList<FootprintPoint>> footprints, out Vector2 mapPosition)
    {
        string? locationName = npc.currentLocation?.Name;
        if (!string.IsNullOrEmpty(locationName) && DisplayContext.AllowsLocation(locationName) && _projection.TryProject(locationName, npc.Tile, out mapPosition))
            return true;

        if (NpcMapPositionResolver.TryGetLatestTrackedPosition(npc.Name, footprints, out FootprintPoint latestPoint))
        {
            if (!DisplayContext.AllowsLocation(latestPoint.LocationName))
            {
                mapPosition = Vector2.Zero;
                return false;
            }

            return _projection.TryProject(latestPoint.LocationName, latestPoint.TilePosition, out mapPosition);
        }

        if (string.IsNullOrEmpty(locationName))
        {
            mapPosition = Vector2.Zero;
            return false;
        }

        mapPosition = Vector2.Zero;
        return false;
    }

    private MapScreenTransform CreateMapScreenTransform()
    {
        return new MapScreenTransform(Vector2.Zero, 1f, _backgroundRenderer.VanillaMapBounds);
    }

    private MapContentTransform? CreateContentTransform()
    {
        if (_contentViewport is null)
            return null;

        return new MapContentTransform(
            _contentViewport.ContentBounds,
            _contentViewport.MapCoordinateBounds,
            _contentViewport.PanOffset,
            _contentViewport.Zoom);
    }

    private Func<Vector2, Vector2> CreateMapToScreenFunc()
    {
        MapContentTransform? contentTransform = CreateContentTransform();
        if (_contentViewport?.IsZoomed == true && contentTransform.HasValue)
            return contentTransform.Value.MapToScreen;

        MapScreenTransform screenTransform = CreateMapScreenTransform();
        return screenTransform.MapToScreen;
    }

    private void UpdateContentViewport()
    {
        if (!_backgroundRenderer.ContentBounds.HasValue)
            return;

        Rectangle mapCoordinateBounds = new(0, 0, _backgroundRenderer.ContentBounds.Value.Width, _backgroundRenderer.ContentBounds.Value.Height);

        if (_contentViewport is null)
            _contentViewport = new MapContentViewportState(_backgroundRenderer.ContentBounds.Value, mapCoordinateBounds);
        else
            _contentViewport.SetContentBounds(_backgroundRenderer.ContentBounds.Value, mapCoordinateBounds);

        if (_pendingScrollDirection != 0)
        {
            _contentViewport.ZoomBy(_pendingScrollDirection);
            _pendingScrollDirection = 0;
        }
    }

    private void PollScrollWheel()
    {
        int current = Mouse.GetState().ScrollWheelValue;
        if (!_lastScrollWheelValue.HasValue)
        {
            _lastScrollWheelValue = current;
            return;
        }

        int delta = current - _lastScrollWheelValue.Value;
        _lastScrollWheelValue = current;
        _lastScrollWheelDelta = delta;
        if (delta != 0)
            ApplyScrollZoom(delta);
    }

    private void ApplyScrollZoom(int direction)
    {
        if (_contentViewport is null)
            _pendingScrollDirection += direction;
        else
            _contentViewport.ZoomBy(direction);
    }

    private void DrawDebugOverlay(SpriteBatch b)
    {
        if (!_shouldShowDebugOverlay() && !Game1.oldKBState.IsKeyDown(Keys.LeftShift))
            return;

        string text = string.Join(Environment.NewLine,
            "Marauder Debug",
            $"Tracked NPCs: {_stats.GetTrackedNpcCount()}",
            $"Selected: {_predictionSelection.SelectedNpcName ?? "none"}",
            $"Footprints: {(_areFootprintsEnabled() ? "enabled" : "disabled")}",
            $"Friendship colors: {(_areFriendshipColorsEnabled() ? "enabled" : "disabled")}",
            $"Zoom: {(_contentViewport?.Zoom ?? 1f):0.00}",
            $"Pan: {(_contentViewport?.PanOffset.X ?? 0):0},{(_contentViewport?.PanOffset.Y ?? 0):0}",
            $"Anchor: {(_contentViewport?.Anchor.X ?? 0):0},{(_contentViewport?.Anchor.Y ?? 0):0}",
            $"Bounds: {_contentViewport?.ContentBounds.ToString() ?? "none"}",
            $"MapCoords: {_contentViewport?.MapCoordinateBounds.ToString() ?? "none"}",
            $"NpcCoords: {_lastNpcCoordinateBounds?.ToString() ?? "none"}",
            $"Wheel: {(_lastScrollWheelValue?.ToString() ?? "none")} d={_lastScrollWheelDelta}",
            $"Source: {_contentRenderer.LastSourceRectangle?.ToString() ?? "none"} | {_contentRenderer.TextureStatus}",
            $"Last record time: {_stats.GetLastRecordedTime()}"
        );

        Vector2 position = new(32, Math.Max(120, Game1.uiViewport.Height - 360));
        b.DrawString(Game1.smallFont, text, position + new Vector2(1, 1), Color.Black * 0.9f);
        b.DrawString(Game1.smallFont, text, position, Color.LightGreen);
    }

}
