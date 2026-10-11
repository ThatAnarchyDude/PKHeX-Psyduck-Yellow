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

These 0.1.0 emulator results did not establish physical-device compatibility.
Later device reports and Android emulator checks are recorded below.

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

## Physical-device report and 0.1.1 compatibility build

The owner subsequently reported that 0.1.0 closed immediately on their Android
16 Samsung phone. The emulator result above does not establish physical-device
compatibility. No crash stack trace from that device is available yet.

Version 0.1.1 uses the Mono interpreter, disables generated native marshal
methods and assembly-store mapping, and adds managed startup diagnostics.
These are compatibility changes, not a proven diagnosis of the device crash.
Failures before the managed activity starts still need Android crash logging.

- Local Release build: passed with zero warnings/errors.
- Valid v2/v3 signature; certificate matches the published 0.1.0 APK.
- Version code 2; minimum SDK 26; target SDK 36.
- ZIP alignment and all 191 packaged ARM64 ELF libraries passed 16 KB checks.
- Published APK SHA256: `e457b27b7412d884b6573e03efb23aa1b5a093964e8ea84da6f1808d65ee08c9`.
- [Expanded Android 16 verification run](https://github.com/ThatAnarchyDude/PKHeX-Psyduck-Yellow/actions/runs/38180556331): includes five cold starts and diagnostic sharing, plus save import/edit/export.
- Owner's phone: subsequently confirmed 0.1.1 works, with ARM64 home-screen diagnostics and in-game screenshots.

## 0.1.2 party management, held items and moves

- Release build passed with zero warnings/errors; valid v2/v3 signature, same
  update certificate and version code 3.
- All 191 packaged ARM64 ELF libraries and APK ZIP alignment passed 16 KB checks.
- **94 save-model assertions passed**, including .sav/.dsv reloads, party size,
  slot compaction, untouched members, shiny/gender/nature consistency, items,
  move PP and unchanged quest/Pokédex state. A seventh party member, removal
  of the last member, incompatible gender, unsupported species, duplicate
  moves and key-item holding are rejected.
- An edited private prototype-0022 save loaded through native DS Continue.
  RAM and the party screen showed three members. The added female shiny Eevee's
  summary displayed Leftovers correctly. No state/RAM edits were used for this check.
- [Expanded Android interface checks](https://github.com/ThatAnarchyDude/PKHeX-Psyduck-Yellow/actions/runs/38184727927) exercise party creation/removal, held-item assignment, move editing and exported-save validation.
- APK SHA256: `c69cf74b250e7b415ad94673baf7dce95cac2a6c92763a11aa0235d972a0b8df`.
- Version 0.1.2 retains the interpreter/runtime configuration confirmed by the
  owner in 0.1.1; the updated interface still needs their physical-device test.
