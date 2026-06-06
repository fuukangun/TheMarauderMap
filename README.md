# The Marauder's Map

[![Stardew Valley](https://img.shields.io/badge/Stardew%20Valley-1.6%2B-brightgreen)](https://www.stardewvalley.net/)
[![SMAPI](https://img.shields.io/badge/SMAPI-4.0%2B-blue)](https://smapi.io/)

中文 | [English](README_EN.md)

**The Marauder's Map** 是一个受《哈利波特》活点地图启发的星露谷物语模组。按下快捷键即可打开一张独立地图，实时查看 NPC 的位置、姓名、好感度颜色和最近足迹。

---

## 目录

- [功能特性](#功能特性)
- [工作原理](#工作原理)
- [安装](#安装)
- [使用方法](#使用方法)
- [配置](#配置)
- [兼容性](#兼容性)
- [素材与致谢](#素材与致谢)
- [常见问题](#常见问题)

---

## 功能特性

### 核心功能

| 功能 | 说明 |
|------|------|
| 独立活点地图 | 默认按 **H** 打开地图，不替换原版 `M` 键地图。 |
| 实时 NPC 位置 | 地图打开后游戏时间不暂停，可以看到 NPC 继续移动。 |
| NPC 姓名显示 | 中文游戏显示本地化 NPC 姓名，其他语言显示英文内部名。 |
| 好感度颜色 | 可按好感度心数给 NPC 姓名着色。 |
| 配偶爱心 | 当 NPC 是玩家配偶时，名字后显示红色爱心。 |
| 活点地图脚印 | 显示 NPC 最近足迹，使用双脚脚印素材并沿移动方向旋转。 |

### 好感度颜色

开启 `EnableFriendshipColors` 后，NPC 姓名颜色会根据玩家与该 NPC 的好感度心数变化。颜色越偏暖表示关系越低，越偏冷/特殊表示关系越高。

| 好感度心数 | 姓名颜色 | RGB |
|------------|----------|-----|
| 0-1 心 | 红色 | `255, 68, 68` |
| 2-3 心 | 橙色 | `255, 136, 68` |
| 4-6 心 | 黄色 | `255, 204, 68` |
| 7-9 心 | 绿色 | `136, 204, 68` |
| 10-12 心 | 青绿色 | `68, 204, 136` |
| 13-14 心 | 紫色 | `204, 136, 255` |

心数会被限制在 0-14 范围内；如果关闭 `EnableFriendshipColors`，NPC 姓名会使用默认的小麦色。

### 足迹显示

| 状态 | 效果 |
|------|------|
| 默认地图 | 每个 NPC 只显示最近 **2** 个足迹点，避免画面过乱。 |
| 点击 NPC 姓名 | 该 NPC 显示最近 **12** 个足迹点。 |
| 新旧足迹 | 最新足迹更深，旧足迹更透明。 |
| 跨地图移动 | 过滤不合理的传送断点，避免异常连线。 |

### 其他特性

- **GMCM 支持**: 集成 [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098)，可在游戏内调整设置
- **双语配置**: GMCM 描述会根据游戏语言显示中文或英文
- **地图缩放与拖动**: 支持鼠标滚轮缩放，放大后拖动地图内容
- **滚轮拦截**: 地图打开时滚轮只用于缩放，不切换物品栏
- **魔法代价**: 每次成功打开活点地图，随机扣除 **4-8** 点体力

---

## 工作原理

1. 游戏中按下配置的快捷键（默认：**H**）
2. 活点地图以 HUD 覆盖层形式打开，游戏时间继续流动
3. 模组按配置间隔记录 NPC 的地图位置
4. 地图上绘制 NPC 姓名、好感度颜色、配偶爱心和足迹
5. 点击 NPC 姓名可以展开该 NPC 的更多历史足迹

**与原版地图的区别：**

| 项目 | 原版地图 | 本模组 |
|------|----------|--------|
| 打开方式 | `M` 键 | 默认 `H` 键 |
| 游戏时间 | 通常暂停 | 不暂停 |
| NPC 展示 | 原版图标/位置 | 姓名、颜色、足迹 |
| 缩放拖动 | 原版行为 | 独立缩放与拖动 |
| 体力消耗 | 无 | 打开时扣 4-8 点体力 |

---

## 安装

### 前置要求

- [Stardew Valley 1.6+](https://www.stardewvalley.net/)
- [SMAPI 4.0+](https://smapi.io/)
- [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098)（可选，用于游戏内修改设置）

### 步骤

1. 如果还没安装 SMAPI，请先安装
2. 下载 `TheMarauderMap` 最新版本
3. 解压到 `StardewValley/Mods/` 文件夹
4. 通过 SMAPI 启动游戏

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

## 使用方法

| 操作 | 按键/方式 |
|------|-----------|
| 打开/关闭活点地图 | 默认 **H** |
| 缩放地图 | 鼠标滚轮 |
| 拖动地图 | 放大后按住鼠标左键拖动 |
| 选择 NPC | 点击 NPC 姓名 |
| 关闭地图 | 再次按 **H** 或 Escape |

点击 NPC 姓名后，该 NPC 会显示最近 12 个足迹点；再次点击或选择其他 NPC 会切换展示目标。

---

## 配置

### 通过 GMCM（推荐）

游戏内菜单 → **模组选项** → **The Marauder's Map** 调整设置。

### 直接编辑配置文件

编辑 `Mods/TheMarauderMap/config.json`：

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

| 设置 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `EnableFootprints` | bool | `true` | 是否显示 NPC 足迹 |
| `EnableFriendshipColors` | bool | `true` | 是否按好感度给姓名着色；关闭后统一使用默认姓名颜色 |
| `RecordIntervalMinutes` | int | `10` | 每隔多少游戏内分钟记录一次 NPC 位置（支持 10/20/30） |
| `MaxStoredFootprintPoints` | int | `40` | 每个 NPC 最多保存多少个足迹点 |
| `MaxVisibleFootprintPoints` | int | `12` | 选中 NPC 时最多显示多少个足迹点 |
| `OpenMapKey` | keybind | `"H"` | 打开/关闭活点地图的快捷键 |

---

## 兼容性

- **Stardew Valley**: 1.6+
- **SMAPI**: 4.0+
- **多人模式**: 未测试
- **地图类模组**: 与大幅替换世界地图或 NPC 地图位置逻辑的模组可能存在显示差异
- **GMCM**: 可选；未安装时仍可通过 `config.json` 配置

---

## 素材与致谢

- **作者**: fuukangun
- **基于**: [SMAPI](https://smapi.io/)
- **脚印素材**: 来自 [icochi/The-Marauders-Map](https://github.com/icochi/The-Marauders-Map)，源项目使用 MIT License

素材来源说明见 `assets/THIRD_PARTY_ASSETS.md`。

---

## 常见问题

### 为什么打开地图会扣体力？

这是“麻瓜使用魔法道具的代价”。每次成功打开活点地图会随机扣除 4-8 点体力，最低不会低于 0。

### 活点地图会暂停游戏时间吗？

不会。地图打开后游戏时间继续流动，NPC 也会继续移动。

### 为什么默认只显示两个足迹点？

为了避免所有 NPC 的足迹堆在一起导致画面混乱。点击某个 NPC 姓名后，会显示该 NPC 最近 12 个足迹点。

### 如何切换中文和英文？

不需要手动切换。游戏语言是中文时显示中文 NPC 姓名和中文 GMCM 描述；其他语言统一显示英文。

### 可以修改打开地图的按键吗？

可以。在 GMCM 或 `config.json` 中修改 `OpenMapKey`，支持 SMAPI 的按键绑定格式，例如 `"LeftShift + H"`。
