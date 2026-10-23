# Psyduck Yellow data generators

These Python scripts (need `ndspy`) rebuild the game-derived resources from a Psyduck Yellow ROM you supply. They are run from the repository root, with the ROM path as the argument:

- `generate-pkhex-expanded-resources.py <rom.nds>`: species, move, item and ability names, personal data, item pockets.
- `generate-pkhex-expanded-learnsets.py <rom.nds>`: level-up learnsets.
- `generate-pkhex-expanded-encounters.py <rom.nds>`: Kanto wild encounters (grass, surf, rods, rock smash).

The ROM itself is never part of this repository. The scripts expect the checkout to be at `private/PKHeX-PY-current`; edit the paths if yours differs.
