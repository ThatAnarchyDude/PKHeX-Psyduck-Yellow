# Building PKHeX with Psyduck Yellow support

This repository is a full PKHeX (upstream kwsch/PKHeX, rebased on commit a9ff306) with Psyduck Yellow added, plus the modified Auto-Legality Mod in `Auto-Legality-Mod/`. Everything needed to build the released program is here; only the game ROM (which you supply) is needed to regenerate the game-derived data.

## Requirements
- .NET 10 SDK (the program needs the .NET 10 Desktop Runtime to run). Windows is the natural build machine; on Linux/macOS add `-p:EnableWindowsTargeting=true`.
- Python 3 with `ndspy` and `Pillow` only if you regenerate data or pictures (see `tools/psyduck-yellow/README.md`).

## Build the program
```sh
dotnet build PKHeX.WinForms/PKHeX.WinForms.csproj -c Release -p:EnableWindowsTargeting=true \
  -p:TreatWarningsAsErrors=false -p:CodeAnalysisTreatWarningsAsErrors=false -p:EnforceCodeStyleInBuild=false
```
The three `-p:` flags after the first line are needed with current SDK analyzers: they raise errors on a few pre-existing warnings in the upstream files (CA1857, a nullable warning); the build is otherwise clean. Output: `PKHeX.WinForms/bin/Release/net10.0-windows/win-x64/` (PKHeX.exe, PKHeX.dll, PKHeX.Core.dll, PKHeX.Drawing*.dll, QRCoder.dll, `*.deps.json`, `PKHeX.runtimeconfig.json`).

## Build the Auto-Legality Mod plugin
```sh
dotnet build Auto-Legality-Mod/PKHeX.Core.AutoMod/PKHeX.Core.AutoMod.csproj -c Release -p:EnableWindowsTargeting=true -p:TreatWarningsAsErrors=false
```
(the solution file `Auto-Legality-Mod/PKHeX-Plugins.sln` builds the plugin set; the release ships `plugins/AutoModPlugins.dll`). Its Psyduck Yellow support is in `PsyduckYellowSupport.cs`.

## Release layout
A release is the build output above in one folder named `PKHeX-Psyduck-Yellow-vN`, with `plugins/AutoModPlugins.dll`, `LICENSE.txt`, `CODE-SIGNING-POLICY.md` and `README-Psyduck-Yellow.txt`, zipped. No ROMs, saves or game files are included, ever.

## Where the Psyduck Yellow pieces are
- Game data: `PKHeX.Core/Editing/PsyduckYellowExpanded*.cs` (names, labels such as "Brawleon (Starter)", legality view), `PKHeX.Core/Resources/byte/personal/personal_psyduck_expanded`, `.../learnset_psyduck_expanded`, `.../legality/wild/Gen4/encounter_py.pkl`; regenerate with `tools/psyduck-yellow/` from your own ROM.
- Save type: `PKHeX.Core/Saves/SAV4PY.cs`; Gen 5+ import: `PsyduckYellowImport.cs`.
- Pictures: custom species resolve to `b_py<n>`, `b_py<n>s`, `a_py<n>` (`PKHeX.Drawing.PokeSprite/Util/SpriteName.cs`, `PsyduckYellowExpanded.GetDisplaySpecies`), files under `PKHeX.Drawing.PokeSprite/Resources/img/` and entries in `Properties/Resources.resx`.
- Documentation: `PSYDUCK-YELLOW.md`.
