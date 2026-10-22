# Psyduck Yellow 0038 expanded-save editor

This is the complete PKHeX Windows application from the existing custom 0029 fork, updated for the released prototype 0038 expanded engine. Original 0029/retail modes and whole-window scaling remain available. Extract into a new folder and run PKHeX.exe using the .NET 10 Windows Desktop runtime.

Open a COPY of a 0038 battery save (.dsv or raw .sav), not the .nds ROM or an emulator save state. Expanded format detection is automatic; the window title identifies 0038 Expanded Engine. Edit normally and export to a different filename, retaining your original. There is no manual expanded-format toggle because the physical layouts differ. Select the existing 0029 Tools option only when editing a legacy project save.

The expanded profile uses the actual IDs and data extracted from released 0038: 30 Pokemon boxes, expanded species/form/personal data, move names and PP/types, item names and pocket assignment, wide abilities, hidden/swap ability flags, minted stat alignment and Hyper Training bits. The engine's species numbering differs from National Dex numbering (for example engine544 is Victini). Entries include forms/placeholders; not every physical record is a distinct playable species. Unknown custom forms use a neutral preview sprite. The saved Pokemon ID is never replaced with the display sprite ID.

The Bag uses its actual physical capacities: Items245, KeyItems92, TM/HM101, Mail12, Medicine40, Berries64, Balls26, Battle30. Item-PC storage remains the existing ten physical stacks. No additional PC slots or badge expansion are invented by the editor. All existing slots are exposed regardless of badges.

Use the normal Ability selection; a stored value outside the species list remains explicitly selectable and is preserved. Minted stat alignment is exposed on the main tab. Shift-click an IV to toggle its Hyper Training flag, as in later-generation PKHeX. Normal IVs, mint bits, ability flags and shiny leaves are kept separate.

Retail legality catalogs and AutoLegality generation do not describe this expanded game's IDs, encounters or learnsets. Expanded-mode retail legality checking is unavailable; no official legality verdict is manufactured. This package does not include a claim of expanded AutoLegality plugin support. Normal manual editing/exporting is independent of retail legality checking. Keep the existing 0029 editor/plugin installation if needed; this package does not overwrite it.

Opening legacy0029 saves remains supported. This editor does NOT migrate an old campaign save into the expanded0038 layout. Keep old/new saves separate; do not rename a legacy save and assume the game can load it. ROM/game fixes are separate from this editor update.

Validation details are included under validation/. Original player saves, ROMs, credentials and emulator states are excluded. The corresponding source archive and GPL-3.0-or-later license accompany the application. Build source using .NET SDK10: dotnet build PKHeX.WinForms/PKHeX.WinForms.csproj -c Release. Original PKHeX contributor notices are retained.