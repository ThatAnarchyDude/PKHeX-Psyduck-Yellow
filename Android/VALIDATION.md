# Android editor prototype 0.1.0 validation

- .NET Android Release build: passed, zero warnings/errors.
- Save-model regression probe: **54 assertions passed**, using a private copy
  of prototype 0022's normal in-game save and its raw `.sav` equivalent.
- Verified edits survive export/reload; untouched story flags, Pokédex,
  other party members, PID, gender and DSV footer are preserved.
- Verified existing custom Hidden Abilities survive; standard and project shiny
  thresholds are interpreted separately. Invalid inputs are rejected.
- APK signature verification: valid v2/v3 signature.
- APK ZIP alignment and all 10 ARM64 libraries: compatible with 16 KB pages.
- APK includes ARM64, ARM32 and x86-64; min SDK 26, target SDK 36 (Android 16).
- Independent GitHub Actions build: [passed](https://github.com/ThatAnarchyDude/PKHeX-Psyduck-Yellow/actions/runs/38176511011).

The exact Samsung One UI 8.5 device still requires an installation/playtest by
its owner. Android platform emulator validation is recorded separately below.

## Android 16 interface and export checks

[Full Android workflow passed](https://github.com/ThatAnarchyDude/PKHeX-Psyduck-Yellow/actions/runs/38178129659).
The actual installed APK opened the native document picker, returned after
cancellation, imported a generated HGSS test save, changed its party Pokémon
from level 5 to level 6, and exported a new save through Android's document
provider. PKHeX.Core then reopened the exported file and verified its checksum,
Pokémon nickname and level. No user save was uploaded for this test.

The slow software emulator in this cloud machine completed APK installation;
its interface was too slow for reliable interaction. The successful interface
checks above used GitHub's hardware-accelerated Android 16 emulator instead.
