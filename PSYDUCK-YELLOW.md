# PKHeX with built-in Psyduck Yellow prototype 0029 support

This is the complete PKHeX Windows application based on the project's 26.08.26 fork. Its normal editors and game support are retained. Prototype 0029 rules are compiled into PKHeX.Core and the Windows interface; no plugin is required.

## Use

1. Extract the application package and run PKHeX.exe. It requires the .NET 10 Windows Desktop runtime, already installed on the owner's PC.
2. Open a COPY of your prototype 0029 battery save (.sav or .dsv).
3. Check Tools > Psyduck Yellow prototype 0029 before editing Pokemon. The window title identifies the active profile. Select this profile again when opening another project save; it is not written into the save itself.
4. Use PKHeX's normal party, boxes, Pokemon, trainer and inventory editors. Export to a new save path and keep your original backup.

For ordinary games, leave the Psyduck Yellow option unchecked. Selecting the option does not convert a retail save into a Psyduck Yellow campaign save.

The application identifies the profile as GameVersion.PY = 254 and EntityContext.Gen4. Pokemon origin bytes and save ROM codes remain HeartGold (7); 254 is never stored in those fields.

## Built-in project rules

- Recognize level-5 Pallet Town starter gifts: male Psyduck, female Eevee, and either-gender Togepi, in ordinary Poke Balls. Reject incorrect origin data, impossible abilities and impossible moves.
- Permit the game's supported starter Hidden Abilities and their supported evolutions. The normal editor retains the actual ability byte without replacing it or rerolling the PID.
- Recognize Togepi's level-1 Tackle and the additional TM compatibility read from the released 0029 ROM.
- Display Golduck as Water/Psychic only in this profile.
- Recognize the project's starter stone evolution alternatives, including Psyduck's Moon Stone evolution.
- Classify existing Pokemon as shiny at XOR < 16 for this profile, retaining retail generation-4 XOR < 8 outside it. Encounter odds do not create extra fields in a save.
- Name Shiny Charm, Friendship Charm and the seven extra TMs correctly in the inventory editor, with their correct pockets. Preserve the native save format, quest flags, custom PC variables and emulator footer.

Hidden Abilities supported by the game: Psyduck/Golduck (Swift Swim), Eevee (Anticipation), Vaporeon (Hydration), Jolteon (Quick Feet), Flareon (Guts), Umbreon (Inner Focus), Leafeon (Chlorophyll), Glaceon (Ice Body), and Togepi/Togetic/Togekiss (Super Luck). Espeon has no newly invented generation-4 Hidden Ability.

## Verified scope and limitations

Core regressions cover all three starters, normal/hidden abilities, perfect IVs, stone evolutions, wrong-level/location/ball/ability/move rejection, shiny thresholds and retail isolation. A copied 0029 save round-trips with party bytes, quest/PC variables, checksums and emulator footer preserved. Native Windows form tests cover the built-in profile, all twelve HA species through normal editing, shiny classification, inventory names and retail isolation.

Custom starter encounter families are described explicitly. The entire altered Kanto wild/campaign gift encounter catalog is not yet replaced; other custom encounters can still receive retail encounter warnings. These project legality results do not establish retail-game or online-transfer eligibility. The exact final exported save also loaded through the released 0029 game's ordinary Continue menu and returned to the field; no emulator state or RAM writes were used.

No ROMs, player saves or credentials are included. Publication of this editor is authorized; game materials remain private.

## Source and license

The release tag and its source archives contain the complete corresponding application source and tests. Original PKHeX contributor notices and LICENSE are retained (GPL-3.0-or-later). Local base commit: a7eeac285796385e001ed714b53f3f149e823a4c. Local branch: codex/psyduck-yellow-0029-support. Build using .NET SDK 10: dotnet publish PKHeX.WinForms/PKHeX.WinForms.csproj -c Release -p:PublishReadyToRun=false.

## PC item tab (v2)
The Items editor now includes the player''s PC storage in the active Psyduck Yellow profile. Prototype0029 supports ten physical saved stacks, up to999 of each. All ten are editable regardless of badges. PC edits are separate from the Bag. Existing items and story state are preserved; new PC contents are marked initialized so the first login does not overwrite them.

The approved next game prototype will start at20 usable in-game slots and unlock5 per distinct Kanto badge, up to60. PKHeX will expose all60 independently of badges once that game save layout is implemented. That expansion and layout are pending; this editor does not write invented extra rows into0029 saves.


## Whole-interface scaling (v4)

Drag the window edges or use the standard Windows maximize button. Text, controls, menu icons, spacing and Pokemon images enlarge together from their original dimensions. At 150% of the default client size, the interface uses a 150% scale. If width and height grow by different amounts, one uniform factor fits the complete interface without clipping or distorting images; unused space is centered around it. Windows' normal working area keeps the visible taskbar available.

Returning the window to its original size restores original fonts and geometry. Saving/loading and changing display language rebuild the layout at its native size before reapplying zoom, avoiding accumulated rounding and font drift. This scales the live native editor, rather than displaying an enlarged screenshot. Manual resizing is included.