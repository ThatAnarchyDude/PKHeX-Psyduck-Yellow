# Auto-Legality Mod for our PKHeX — Psyduck Yellow

This is the Auto-Legality Mod plugin with built-in support for Psyduck Yellow saves (`SAV4PY`) in our PKHeX fork:
https://github.com/ThatAnarchyDude/PKHeX-Psyduck-Yellow

## Install
Extract `AutoModPlugins.dll` into the `plugins` folder beside PKHeX.exe (Properties > Unblock if Windows blocks it). Do not install another PKHeX.Core.dll: the plugin uses the core inside our editor, and both carry the same version.

## What it does for Psyduck Yellow saves
Legalizing (Tools > Auto-Legality Mod, and Set Legal) builds the Pokémon from the game's own encounters, found by species ID:
- **Starter Psyduck/Eevee line** (1561–1564 and the starter Eeveelutions/Novareon 1565–1582) always come from the Pallet Town starter gift. They are recognised by their own species IDs; nothing else can be them.
- **Togepi** comes from the Pallet gift. Other Kanto wild Pokémon come from the game's wild areas, with the matching met location, level and PID.
- A plain Psyduck or Eevee can't be the starter gift. If the game has no source for a request (e.g. a non-starter Eevee), it fails instead of inventing one.
- The Easter-egg fallback is disabled for these saves. Other games use the normal ALM path.

Not covered yet: Showdown imports can't name the starter species (use Set Legal on the Pokémon itself), static encounters other than the starters, gifts, trades, eggs and Headbutt trees.

## Source and license
Based on https://github.com/santacrab2/PKHeX-Plugins commit 90410f2681a0a72680d12280a1e0f14715e67dff. Original LICENSE and notices are retained. Build with .NET SDK 10:
  dotnet build AutoLegalityMod/AutoModPlugins.csproj -c Release -p:PsyduckYellowCoreProject=/path/to/PKHeX.Core.csproj
