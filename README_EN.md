# The Marauder's Map

[![Stardew Valley](https://img.shields.io/badge/Stardew%20Valley-1.6%2B-brightgreen)](https://www.stardewvalley.net/)
[![SMAPI](https://img.shields.io/badge/SMAPI-4.0%2B-blue)](https://smapi.io/)

[中文](README.md) | English

**The Marauder's Map** is a Stardew Valley mod inspired by the magical map from Harry Potter. Press a hotkey to open a live map showing NPC names, friendship colors, spouse hearts, and recent footprints.

---

## Table of Contents

- [Features](#features)
- [How It Works](#how-it-works)
- [Installation](#installation)
- [Usage](#usage)
- [Configuration](#configuration)
- [Compatibility](#compatibility)
- [Credits](#credits)
- [FAQ](#faq)

---

## Features

### Core Features

| Feature | Description |
|---------|-------------|
| Standalone live map | Press **H** by default to open the map. It does not replace the vanilla `M` map. |
| Real-time NPC positions | Game time keeps running while the map is open, so NPCs continue moving. |
| NPC name display | Chinese games show localized NPC names; other languages show English internal names. |
| Friendship colors | NPC names can be colored by friendship heart level. |
| Spouse heart | If an NPC is your spouse, a red heart appears after their name. |
| Footprint trails | NPCs leave recent two-foot footprint trails that rotate with their movement direction. |

### Friendship Colors

When `EnableFriendshipColors` is enabled, NPC name colors change based on your friendship heart level with that NPC. Warmer colors mean lower friendship; cooler or more distinctive colors mean higher friendship.

| Friendship hearts | Name color | RGB |
|-------------------|------------|-----|
| 0-1 hearts | Red | `255, 68, 68` |
| 2-3 hearts | Orange | `255, 136, 68` |
| 4-6 hearts | Yellow | `255, 204, 68` |
| 7-9 hearts | Green | `136, 204, 68` |
| 10-12 hearts | Teal green | `68, 204, 136` |
| 13-14 hearts | Purple | `204, 136, 255` |

Heart values are clamped to the 0-14 range. If `EnableFriendshipColors` is disabled, NPC names use the default wheat color.

### Footprint Display

| State | Behavior |
|-------|----------|
| Default map | Each NPC shows only the latest **2** footprint points to keep the map readable. |
| Click an NPC name | The selected NPC shows the latest **12** footprint points. |
| New vs old footprints | Newer footprints are darker; older ones are more transparent. |
| Map transitions | Invalid teleport breaks are filtered to avoid strange connection lines. |

### Additional Features

- **GMCM support**: Integrated with [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) for in-game settings
- **Bilingual config text**: GMCM labels switch between Chinese and English based on game language
- **Zoom and pan**: Use the mouse wheel to zoom, then drag the zoomed map content
- **Scroll wheel suppression**: While the map is open, the wheel zooms the map instead of switching toolbar items
- **Magic cost**: Each successful map opening costs **4-8** stamina

---

## How It Works

1. Press the configured hotkey while in-game (default: **H**)
2. The Marauder's Map opens as a HUD overlay, and game time keeps running
3. The mod records NPC map positions at the configured interval
4. The map draws NPC names, friendship colors, spouse hearts, and footprint trails
5. Click an NPC name to show more of that NPC's recent footprint history

**Differences from the vanilla map:**

| Aspect | Vanilla Map | This Mod |
|--------|-------------|----------|
| Hotkey | `M` | Default `H` |
| Game time | Usually paused | Not paused |
| NPC display | Vanilla icons/positions | Names, colors, footprints |
| Zoom/pan | Vanilla behavior | Independent zoom and pan |
| Stamina cost | None | Costs 4-8 stamina when opened |

---

## Installation

### Requirements

- [Stardew Valley 1.6+](https://www.stardewvalley.net/)
- [SMAPI 4.0+](https://smapi.io/)
- [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) (optional, for in-game settings)

### Steps

1. Install SMAPI if you haven't already
2. Download the latest release of `TheMarauderMap`
3. Extract the zip into your `StardewValley/Mods/` folder
4. Launch the game via SMAPI

```
StardewValley/
└── Mods/
    └── TheMarauderMap/
        ├── TheMarauderMap.dll
        ├── manifest.json
        └── assets/
            ├── footprints.png
            ├── footprints-cloud.png
            ├── heart.png
            └── THIRD_PARTY_ASSETS.md
```

---

## Usage

| Action | Control |
|--------|---------|
| Open/close the map | Default **H** |
| Zoom map | Mouse wheel |
| Pan map | Hold left mouse button and drag while zoomed in |
| Select NPC | Click an NPC name |
| Close map | Press **H** again or Escape |

Clicking an NPC name expands that NPC's footprint trail to the latest 12 points. Click another NPC name to switch the selected target.

---

## Configuration

### Via GMCM (Recommended)

Open the in-game menu -> **Mod Options** -> **The Marauder's Map** to adjust settings.

### Via config.json

Edit `Mods/TheMarauderMap/config.json`:

```json
{
  "EnableFootprints": true,
  "EnableFriendshipColors": true,
  "RecordIntervalMinutes": 10,
  "MaxStoredFootprintPoints": 40,
  "MaxVisibleFootprintPoints": 12,
  "OpenMapKey": "H"
}
```

| Setting | Type | Default | Description |
|---------|------|---------|-------------|
| `EnableFootprints` | bool | `true` | Show NPC footprints |
| `EnableFriendshipColors` | bool | `true` | Color NPC names by friendship; disabled uses the default name color |
| `RecordIntervalMinutes` | int | `10` | How often to record NPC positions in in-game minutes (10/20/30) |
| `MaxStoredFootprintPoints` | int | `40` | Maximum stored footprint points per NPC |
| `MaxVisibleFootprintPoints` | int | `12` | Maximum footprint points shown for the selected NPC |
| `OpenMapKey` | keybind | `"H"` | Hotkey to open/close the map |

---

## Compatibility

- **Stardew Valley**: 1.6+
- **SMAPI**: 4.0+
- **Multiplayer**: Not tested
- **Map mods**: Mods that heavily replace the world map or NPC map-position logic may cause display differences
- **GMCM**: Optional; without it, you can still edit `config.json`

---

## Credits

- **Author**: fuukangun
- **Built with**: [SMAPI](https://smapi.io/)
- **Footprint assets**: From [icochi/The-Marauders-Map](https://github.com/icochi/The-Marauders-Map), licensed under the MIT License

See `assets/THIRD_PARTY_ASSETS.md` for third-party asset notes.

---

## FAQ

### Why does opening the map cost stamina?

It is the price a muggle pays for using a magical item. Each successful map opening costs 4-8 stamina and never reduces stamina below 0.

### Does the map pause game time?

No. Game time continues while the map is open, and NPCs continue moving.

### Why are only two footprint points shown by default?

Showing too many footprints for every NPC makes the map noisy. Click an NPC name to show that NPC's latest 12 footprint points.

### How do I switch between Chinese and English?

No manual switch is needed. Chinese game language shows Chinese NPC names and GMCM text; all other languages use English.

### Can I change the map hotkey?

Yes. Change `OpenMapKey` in GMCM or `config.json`. It supports SMAPI keybind syntax, such as `"LeftShift + H"`.
