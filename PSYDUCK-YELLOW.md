# PKHeX with built-in Psyduck Yellow support

A full PKHeX (upstream kwsch/PKHeX, rebased on commit a9ff306) with Psyduck Yellow added as a game of its own. No plugin or profile switch is needed.

Requirements: Windows and the .NET 10 Desktop Runtime.

## Use

1. Run `PKHeX.exe`.
2. Open a COPY of your Psyduck Yellow battery save (`.dsv` or `.sav`). It is detected automatically; the title bar shows "Psyduck Yellow Expanded Engine".
3. Edit, then export to a new file. Keep your original.

## What works

Party, 30 boxes, bag and item PC, Pokedex, trainer data, event flags and variables (with Psyduck Yellow labels), the game's own species/move/item/ability lists (starter and Mega copies labelled), correct move types, and legality checks for Pokemon within the Generation 4 catalogs (HeartGold rules plus the starter rules; level-up moves and Kanto wild encounters come from the game). Psyduck Yellow can be a Pokemon's origin game.

Saves from prototypes 0038 through 0098 have been opened (data regenerated from the 0099 build). Not every editor window has been tested.

## Known limits

- Legality covers Kanto wild encounters (grass, surfing, fishing, Rock Smash) from the game's own tables. Static encounters, gifts, in-game trades, hatched eggs and Headbutt trees are not covered yet, so Pokemon from those can show as illegal.
- TM, HM, egg and tutor moves use HeartGold's data.
- Species types for Sylveon (Fairy) and Novareon (Stellar) are not converted yet.
- Prototype 0029 saves still need Tools > Psyduck Yellow prototype 0029.
- Pokemon outside the Generation 4 catalogs show "Analysis not available".

No ROMs, saves or game files are included. PKHeX is GPL-3.0-or-later; see `LICENSE`.

## Windows security warning

Releases are not code-signed yet, so Windows may show "We can't verify who created this file".

- Run the app from a normal folder on a local drive (for example `C:\Tools\PKHeX`), not from a network path such as `\\wsl.localhost\...`; Windows treats network paths as untrusted.
- For a downloaded zip: right-click the zip, choose Properties, tick **Unblock**, then extract.

Free open-source code signing through SignPath Foundation is being prepared; see [CODE-SIGNING-POLICY.md](CODE-SIGNING-POLICY.md).
