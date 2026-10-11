# Pokémon Psyduck Yellow — PKHeX project fork

This is the project's fork of [official PKHeX](https://github.com/kwsch/PKHeX).
It retains the upstream application and license. Project-specific compatibility
currently lives in a separate plugin, so users can keep using standard PKHeX.
The fork provides an upstream reference and a place for any future application
changes that cannot be supported through the plugin interfaces.

- [Plugin source, instructions and tests](https://github.com/ThatAnarchyDude/psyduck-yellow-pkhex-plugin)
- [Experimental plugin downloads](https://github.com/ThatAnarchyDude/psyduck-yellow-pkhex-plugin/releases)
- [Game development log](https://github.com/ThatAnarchyDude/corvus-workshop/blob/main/psyduck-yellow/DEVELOPMENT-LOG.md)

The initial plugin targets PKHeX **26.08.26**, upstream commit
`66ef5983a5d349efa2dd615f2926a1b1ee97306e`. It supports the tested HeartGold save
format, project shiny classification, supported Hidden Ability edits and
player PC stacks. See the plugin's instructions for tested scope and limits.
This documentation does not represent a modified application release.

PKHeX's upstream license and contributor notices remain in place. Its Core
package declares GPL-3.0-or-later; project plugin/probe source uses that license.
No ROMs, player saves or credentials are distributed through these repositories.

## Android save editor

A native Android interface for Psyduck Yellow and HeartGold/SoulSilver emulator
saves is being developed in [Android](Android/README.md). It reuses PKHeX.Core
and is independent of the Windows plugin. Initial features focus on party edits
and safe save-copy export.
