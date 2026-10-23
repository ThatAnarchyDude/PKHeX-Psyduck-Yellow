"""Export the expanded game's level-up learnsets (ROM a/0/3/3, 136 bytes per species) for PKHeX legality.
Usage: python -I private/generate-pkhex-expanded-learnsets.py [rom.nds]
Format of the output: u16 species count; per species: u8 n, n*u16 move, n*u8 level."""
import struct,sys
from pathlib import Path
import ndspy.rom,ndspy.narc
rom=sys.argv[1] if len(sys.argv)>1 else '/mnt/c/RetroArch-Win64/downloads/Pokemon-Psyduck-Yellow-0044-novareon.nds'
blob=ndspy.narc.NARC(ndspy.rom.NintendoDSRom.fromFile(rom).getFileByName('a/0/3/3')).files[0]
SLOT=136;count=len(blob)//SLOT;assert len(blob)==count*SLOT
out=bytearray(struct.pack('<H',count))
for sp in range(count):
    rows=[]
    for k in range(SLOT//4):
        move,level=struct.unpack_from('<HH',blob,sp*SLOT+k*4)
        if move==0xFFFF:break
        assert level<=100 and move<0x8000,(sp,move,level)
        rows.append((move,level))
    out+=struct.pack('<B',len(rows))+b''.join(struct.pack('<H',m) for m,_ in rows)+bytes(l for _,l in rows)
dest=Path('private/PKHeX-PY-current/PKHeX.Core/Resources/byte/learnset_psyduck_expanded')
dest.write_bytes(out);print(count,'species',len(out),'bytes ->',dest)
