# Psyduck Yellow data generators

Everything here is self-contained (Python 3 with `ndspy`, plus `Pillow` for the art tools). The scripts rebuild the game-derived resources from a Psyduck Yellow ROM you supply (the ROM is never part of this repository). Run them from the repository root; they find the repository from their own location:

```sh
python -m pip install ndspy pillow
python -I tools/psyduck-yellow/generate-pkhex-expanded-resources.py  <rom.nds>   # names, personal data, abilities, forms, item pockets -> PKHeX.Core/Editing/PsyduckYellowExpanded.Resources.cs + Resources/byte/personal/personal_psyduck_expanded
python -I tools/psyduck-yellow/generate-pkhex-expanded-learnsets.py  <rom.nds>   # level-up learnsets -> Resources/byte/learnset_psyduck_expanded
python -I tools/psyduck-yellow/generate-pkhex-expanded-encounters.py <rom.nds>   # Kanto wild encounters -> Resources/legality/wild/Gen4/encounter_py.pkl
```

Run on the v8 ROM (BETA 0.1.57, build 0157: 1587 species rows, 3149 items, Mega Typhlosion and Typhlosionite) they reproduce the committed files byte for byte. `msgdecode.py` and `text-map.json` are the game's message-bank decoder and character table.

`art/` holds the picture tools used for the custom species (box and artwork sprites for Brawleon, Venomeon, Duneon, Zephyreon, Chryseon, Strateon, Wraitheon, Wyrmeon, Ferreon and Stellareon): `eeveelution_art.py` (cut-outs of the concept boards into battle sprites) and `pkhex_icons.py` (68x56 box/artwork pictures, normal and shiny). They need the concept boards (repository Psyduck-Yellow-ExMons) and the `nitrogfx` tool (built from pokeheartgold's `tools/nitrogfx`); the resulting PNGs are already committed under `PKHeX.Drawing.PokeSprite/Resources/img/`, so they only matter if you want to change the art. See `BUILDING-PSYDUCK-YELLOW.md`.
