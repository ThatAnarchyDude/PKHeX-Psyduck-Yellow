"""Export the expanded game's Kanto wild encounters for PKHeX legality.
Usage: python -I private/generate-pkhex-expanded-encounters.py <rom.nds> [--report]
Reads each Kanto map's wild table (ROM a/0/3/7, 196 bytes; id = map header byte 0, location = header byte 18) and writes
PKHeX's encounter_py.pkl (BinLinker 'py'): per area u8 location, 0, u8 slot type, u8 rate, u16 ground tile, then 10-byte slots
(u16 species, form, slot number, min, max, magnet-pull index/count, static index/count).
Swarm and radio slots are skipped (the game replaces them with the area's own species). Headbutt trees are not exported."""
import struct,sys
from pathlib import Path
import ndspy.rom,ndspy.narc
TABLE=0xF6BE0  # map header table in ARM9 section 0 (24 bytes per map)
rom=ndspy.rom.NintendoDSRom.fromFile(sys.argv[1])
arm9=bytes(rom.loadArm9().sections[0].data)
wild=ndspy.narc.NARC(rom.getFileByName('a/0/3/7')).files
personal=ndspy.narc.NARC(rom.getFileByName('a/0/0/2')).files
STEEL,ELECTRIC=8,13  # game-stored type numbers
def types(sp): return (personal[sp][6],personal[sp][7]) if sp<len(personal) else (0,0)
# retail ground tiles by (location, slot type) from PKHeX's own HeartGold data
retail={}
d=Path('private/PKHeX-PY-current/PKHeX.Core/Resources/legality/wild/Gen4/encounter_hg.pkl').read_bytes()
cnt=struct.unpack_from('<H',d,2)[0];offs=struct.unpack_from('<%dI'%(cnt+1),d,4)
for i in range(cnt):
    a=d[offs[i]:offs[i+1]];retail[(a[0],a[2])]=struct.unpack_from('<H',a,4)[0]
DEFAULT_TILE={0:0x4,1:0x80,2:0x80,3:0x80,4:0x80,5:0x10}  # grass, water, rods, rock
SLOT_RATE={0:0,1:1,5:2,2:3,3:4,4:5}  # slot type -> rate byte index in the ROM table

def slots_with_lures(rows):
    """rows: [(species, slot, min, max)]; add Magnet Pull (Steel) and Static (Electric) indices for one time period."""
    steel=[i for i,r in enumerate(rows) if STEEL in types(r[0])]
    elec=[i for i,r in enumerate(rows) if ELECTRIC in types(r[0])]
    out=[]
    for i,(sp,slot,lo,hi) in enumerate(rows):
        mpi=steel.index(i) if i in steel else 0;mpc=len(steel) if i in steel else 0
        sti=elec.index(i) if i in elec else 0;stc=len(elec) if i in elec else 0
        out.append(struct.pack('<HBBBBBBBB',sp,0,slot,lo,hi,mpi,mpc,sti,stc))
    return out

def areas_for(wid):
    b=wild[wid]
    if len(b)!=196:return
    lv=list(b[8:20])
    # grass: morning, day, night tables concatenated
    if b[0]:
        rows=[]
        for period in range(3):
            sps=struct.unpack_from('<12H',b,20+24*period)
            prow=[(sp,i,lv[i],lv[i]) for i,sp in enumerate(sps) if sp]
            rows.append(prow)
        yield 0,b[0],[s for p in rows for s in slots_with_lures(p)]
    def table(off,n):
        return [struct.unpack_from('<BBH',b,off+4*i) for i in range(n)]
    for typ,off,n in ((1,100,5),(5,120,2),(2,128,5),(3,148,5),(4,168,5)):
        rate=b[SLOT_RATE[typ]]
        if not rate:continue
        rows=[(sp,i,lo,hi) for i,(lo,hi,sp) in enumerate(table(off,n)) if sp]
        if rows:yield typ,rate,slots_with_lures(rows)

result={};used=set()
for m in range(0,720):
    at=TABLE+m*24
    if at+24>len(arm9) or not arm9[at+20]&1:continue
    wid=arm9[at];loc=arm9[at+18]
    if wid>=len(wild) or loc==0:continue
    for typ,rate,slots in areas_for(wid):
        if any(struct.unpack_from('<H',s)[0]>493 for s in slots):
            slots=[s for s in slots if struct.unpack_from('<H',s)[0]<=493]
        if not slots:continue
        key=(loc,typ,b''.join(slots))
        if key in used:continue
        used.add(key)
        tile=retail.get((loc,typ),DEFAULT_TILE[typ])
        result[(loc,typ,len(result))]=struct.pack('<BBBBH',loc,0,typ,rate,tile)+b''.join(slots)
items=[v for _,v in sorted(result.items())]
head=struct.pack('<2sH',b'py',len(items));pos=4+(len(items)+1)*4;offsets=[]
for v in items:offsets.append(pos);pos+=len(v)
offsets.append(pos)
blob=head+struct.pack('<%dI'%len(offsets),*offsets)+b''.join(items)
dest=Path('private/PKHeX-PY-current/PKHeX.Core/Resources/legality/wild/Gen4/encounter_py.pkl');dest.write_bytes(blob)
print(len(items),'areas',len(blob),'bytes ->',dest)
if '--report' in sys.argv:
    from collections import Counter
    print(Counter(k[1] for k in result), 'locations',len({k[0] for k in result}))
