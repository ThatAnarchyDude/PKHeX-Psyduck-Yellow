from pathlib import Path
import sys,json,struct,ndspy.rom,ndspy.narc
sys.path.insert(0,'yellow-heartgold/tools');from prototype import decode_messages
root=Path('private/PKHeX-PY-current/PKHeX.Core');out=Path('yellow-heartgold/build/0056-pkhex-v1');rom=ndspy.rom.NintendoDSRom.fromFile(sys.argv[1])
text=ndspy.narc.NARC(rom.getFileByName('a/0/2/7'));mapping=json.loads(Path('yellow-heartgold/tools/text-map.json').read_text());chars={v:k for k,v in mapping.items()}
def names(i):
 _,lines=decode_messages(text.files[i]);return [''.join(chars.get(v,'?' if v not in (0xffff,0xe000) else '') for v in row if v!=0xffff) for row in lines]
personal=ndspy.narc.NARC(rom.getFileByName('a/0/0/2')).files;items=ndspy.narc.NARC(rom.getFileByName('a/0/1/7')).files;addons=ndspy.narc.NARC(rom.getFileByName('a/0/2/8')).files
assert len(personal)>=1565 and all(len(v)==44 for v in personal);assert len(addons[7])>=len(personal)*2
species,moves,abilities,itemnames=[names(i) for i in [237,750,720,222]]
pockets=[[] for _ in range(8)]
for i,v in enumerate(items):
 if i and any(v):
  pocket=(struct.unpack_from('<H',v,8)[0]>>7)&15
  if pocket<8:pockets[pocket].append(i)
# Metadata is derived from released0038, never from retail national-index assumptions.
abilities12=[struct.unpack_from('<2H',v,22) for v in personal];hidden=list(struct.unpack_from('<'+'H'*(len(addons[7])//2),addons[7]))[:len(personal)]
forms=list(struct.unpack_from('<'+'H'*(len(addons[11])//2),addons[11]));converted=bytearray()
for v in personal:
 b=bytearray(v);b[22]=v[22];b[23]=v[24];b[24]=v[26];b[25]=v[27];b[26:44]=bytes(18);converted+=b
res=root/'Resources/byte/personal/personal_psyduck_expanded';res.write_bytes(converted)
code='using System;\nnamespace PKHeX.Core;\npublic static partial class PsyduckYellowExpanded\n{\n'
for label,data in [('SpeciesNames',species),('MoveNames',moves),('AbilityNames',abilities),('ItemNames',itemnames)]:code+='    public static readonly string[] '+label+' = '+json.dumps(data,ensure_ascii=True).replace('\\u0000','')+';\n'
for label,data in [('Ability1',[v[0] for v in abilities12]),('Ability2',[v[1] or v[0] for v in abilities12]),('HiddenAbility',hidden),('FormMap',[v&0x7fff for v in forms])]:code+='    public static readonly ushort[] '+label+' = ['+','.join(map(str,data))+'];\n'
code+='    public static readonly ushort[][] ItemPockets = [\n'+',\n'.join('        ['+','.join(map(str,vs))+']' for vs in pockets)+'\n    ];\n}\n'
(root/'Editing/PsyduckYellowExpanded.Resources.cs').write_text(code)
p=root/'Editing/PsyduckYellowExpanded.cs';p.write_text(p.read_text(encoding='utf-8-sig').replace('public static class PsyduckYellowExpanded','public static partial class PsyduckYellowExpanded'))
report={'species_records':len(personal),'moves':len(moves),'abilities':len(abilities),'items':len(itemnames),'pockets':[len(v) for v in pockets],'last_species':species[-1],'last_item':itemnames[-1],'form_table_entries':len(forms),'personal_resource_bytes':len(converted),'profile_source':'0044 novareon checkpoint'};(out/'resource-report.json').write_text(json.dumps(report,indent=2));print(json.dumps(report))
