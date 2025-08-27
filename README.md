# IPD Adjuster

A (for now until I can find a better way) [BepInEx 5](https://docs.bepinex.dev/v5.4.21/articles/user_guide/installation/index.html) Mod and a [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader) mod for [Resonite](https://resonite.com/).
<br>The BepInEx 5 mod is the actual mod, whereas the ResoniteMod is for config. Both need each other to work properly.

This mod allows you to adjust your IPD via a multiplier in the Mod's Settings

## Installation
1. Install [BepInEx 5](https://github.com/BepInEx/BepInEx/releases/download/v5.4.23.3/BepInEx_win_x64_5.4.23.3.zip) to the Renderer folder in Resonite. Usually `C:\Program Files (x86)\Steam\steamapps\common\Resonite\Renderer`
2. Place the [IPDAdjuster_BepInEx.dll](https://github.com/ErrorJan/ResoniteMod-FixMirrorSteamVRIPDOffset/releases/latest/download/FixMirrorSteamVRIPDOffset_BepInEx.dll) in the BepInEx/plugins folder. Usually `C:\Program Files (x86)\Steam\steamapps\common\Resonite\Renderer\BepInEx\plugins`
3. Install [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader).
4. Place [IPDAdjuster.dll](https://github.com/ErrorJan/ResoniteMod-IPDAdjuster/releases/latest/download/IPDAdjuster.dll) into your `rml_mods` folder. This folder should be at `C:\Program Files (x86)\Steam\steamapps\common\Resonite\rml_mods` for a default install. You can create it if it's missing, or if you launch the game once with ResoniteModLoader installed it will create this folder for you.
5. Start the game. If you want to verify that the mod is working you can check your Resonite logs.
