#!/usr/bin/env python3
"""Exercise the real installed Android app and system file picker via adb."""
from pathlib import Path
import re
import subprocess
import time
import xml.etree.ElementTree as ET

root = Path(__file__).resolve().parents[2]
output = root / 'Android/artifacts/smoke'
output.mkdir(parents=True, exist_ok=True)

def adb(*args):
    return subprocess.check_output(['adb', *args], timeout=90)

def xml():
    adb('shell', 'uiautomator', 'dump', '/sdcard/psyduck-window.xml')
    return adb('shell', 'cat', '/sdcard/psyduck-window.xml')

apks = list((root / 'Android/artifacts').glob('Psyduck-Save-Editor-*.apk'))
if len(apks) != 1:
    raise RuntimeError('Expected exactly one APK for this test run.')
adb('install', '-r', str(apks[0]))
adb('shell', 'am', 'start', '-n', 'org.psyduckyellow.saveeditor/crc6475fe615b484c1a11.MainActivity')
for attempt in range(12):
    time.sleep(2)
    screen = xml()
    (output / 'last-screen.xml').write_bytes(screen)
    if b'open save' in screen.lower():
        break
else:
    (output / 'failed-home.png').write_bytes(adb('exec-out', 'screencap', '-p'))
    (output / 'logcat.txt').write_bytes(adb('logcat', '-d'))
    raise RuntimeError('Editor home screen did not appear; see captured screen/logcat.')
(output / 'home.xml').write_bytes(screen)
(output / 'home.png').write_bytes(adb('exec-out', 'screencap', '-p'))
node = next(n for n in ET.fromstring(screen).iter('node') if n.attrib.get('text', '').lower() == 'open save')
coords = list(map(int, re.findall(r'\d+', node.attrib['bounds'])))
adb('shell', 'input', 'tap', str((coords[0]+coords[2])//2), str((coords[1]+coords[3])//2))
time.sleep(3)
picker = xml()
(output / 'file-picker.xml').write_bytes(picker)
(output / 'file-picker.png').write_bytes(adb('exec-out', 'screencap', '-p'))
if b'com.android.documentsui' not in picker:
    raise RuntimeError('Android system file picker did not open.')
adb('shell', 'input', 'keyevent', 'BACK')
time.sleep(2)
if b'open save' not in xml().lower():
    raise RuntimeError('Editor did not resume after cancelling file selection.')
# Exercise the new support path and repeated cold launches reported by the owner.
node = next(n for n in ET.fromstring(xml()).iter('node') if n.attrib.get('text', '').lower() == 'share startup diagnostics')
coords = list(map(int, re.findall(r'\d+', node.attrib['bounds'])))
adb('shell', 'input', 'tap', str((coords[0]+coords[2])//2), str((coords[1]+coords[3])//2))
time.sleep(2)
chooser = xml()
(output / 'diagnostic-share.xml').write_bytes(chooser)
if b'org.psyduckyellow.saveeditor' in chooser and b'open save' in chooser.lower():
    raise RuntimeError('Startup diagnostic share chooser did not open.')
adb('shell', 'input', 'keyevent', 'BACK')
for attempt in range(5):
    adb('shell', 'am', 'force-stop', 'org.psyduckyellow.saveeditor')
    adb('shell', 'am', 'start', '-n', 'org.psyduckyellow.saveeditor/crc6475fe615b484c1a11.MainActivity')
    time.sleep(3)
    if b'open save' not in xml().lower():
        (output / 'logcat.txt').write_bytes(adb('logcat', '-d'))
        raise RuntimeError(f'Editor failed cold launch {attempt + 1}.')
(output / 'result.txt').write_text('PASS: Android 16 APK installation, app launch, system file picker, cancellation/resume.\n')
print((output / 'result.txt').read_text())

# Complete an edit through Android's document providers using synthetic data.
fixture = root / 'Android/artifacts/synthetic-test.sav'
if fixture.exists():
    def nodes():
        return list(ET.fromstring(xml()).iter('node'))

    def tap(node):
        coords = list(map(int, re.findall(r'\d+', node.attrib['bounds'])))
        adb('shell', 'input', 'tap', str((coords[0]+coords[2])//2), str((coords[1]+coords[3])//2))

    def find(text, scroll=False):
        for attempt in range(12):
            current = nodes()
            for node in current:
                if node.attrib.get('text', '').lower() == text.lower():
                    return node
            if scroll:
                adb('shell', 'input', 'swipe', '160', '500', '160', '160', '250')
            time.sleep(1)
        (output / 'failure.xml').write_bytes(xml())
        (output / 'failure.png').write_bytes(adb('exec-out', 'screencap', '-p'))
        raise RuntimeError('Could not locate Android control: ' + text)

    adb('push', str(fixture), '/sdcard/Download/synthetic-test.sav')
    tap(find('Open save'))
    time.sleep(2)
    roots = next(n for n in nodes() if n.attrib.get('content-desc', '').lower() == 'show roots')
    tap(roots)
    tap(find('Downloads'))
    tap(find('synthetic-test.sav'))
    for attempt in range(8):
        screen = xml()
        if b'Trainer: TEST' in screen:
            break
        time.sleep(2)
    else:
        (output / 'import-failure.xml').write_bytes(screen)
        (output / 'import-failure.png').write_bytes(adb('exec-out', 'screencap', '-p'))
        raise RuntimeError('Save import did not show the synthetic trainer.')
    (output / 'import.png').write_bytes(adb('exec-out', 'screencap', '-p'))
    field = None
    for attempt in range(12):
        current = nodes()
        for i, node in enumerate(current):
            if node.attrib.get('text', '') == 'Level (1–100)' and i+1 < len(current):
                candidate = current[i+1]
                if candidate.attrib.get('class') == 'android.widget.EditText':
                    field = candidate
                    break
        if field is not None:
            break
        adb('shell', 'input', 'swipe', '160', '500', '160', '160', '250')
    if field is None:
        raise RuntimeError('Level edit field did not appear.')
    tap(field)
    adb('shell', 'input', 'keyevent', 'KEYCODE_MOVE_END')
    adb('shell', 'input', 'keyevent', 'KEYCODE_DEL')
    adb('shell', 'input', 'text', '6')
    ime = adb('shell', 'dumpsys', 'input_method').lower()
    if b'inputshown=true' in ime or b'inputshown: true' in ime:
        adb('shell', 'input', 'keyevent', 'BACK')
    def labeled_field(label):
        for attempt in range(15):
            current = nodes()
            for i, node in enumerate(current):
                if node.attrib.get('text', '') == label and i+1 < len(current):
                    candidate = current[i+1]
                    if candidate.attrib.get('class', '').endswith(('EditText', 'AutoCompleteTextView')):
                        return candidate
            adb('shell', 'input', 'swipe', '160', '500', '160', '160', '250')
        raise RuntimeError('Could not locate field: ' + label)

    def replace_field(node, value):
        tap(node)
        adb('shell', 'input', 'keyevent', 'KEYCODE_MOVE_END')
        old = node.attrib.get('text', '')
        if old: adb('shell', 'input', 'keyevent', *(['KEYCODE_DEL'] * len(old)))
        adb('shell', 'input', 'text', value)
        for _ in range(2):
            ime = adb('shell', 'dumpsys', 'input_method').lower()
            if b'inputshown=true' not in ime and b'inputshown: true' not in ime:
                break
            adb('shell', 'input', 'keyevent', 'BACK')

    def top():
        for _ in range(7): adb('shell', 'input', 'swipe', '160', '160', '160', '600', '150')

    replace_field(labeled_field('Held item (name or ID; None removes it)'), '234')
    top(); tap(find('Add Pokémon', scroll=True))
    replace_field(labeled_field('Species (name or National Dex ID)'), '133')
    tap(find('Add'))
    time.sleep(2)
    tap(find('Add Pokémon', scroll=True)); tap(find('Add'))
    time.sleep(2)
    tap(find('Remove selected Pokémon', scroll=True)); tap(find('Remove'))
    time.sleep(2)
    tap(find('Edit moves', scroll=True))
    replace_field(labeled_field('Move 1 (name or ID; None clears it)'), '98')
    tap(find('Apply moves'))
    time.sleep(2)
    (output / 'party-edited.png').write_bytes(adb('exec-out', 'screencap', '-p'))
    tap(find('Export edited copy', scroll=True))
    tap(find('Save'))
    time.sleep(2)
    adb('pull', '/sdcard/Download/synthetic-test-edited.sav', str(output / 'ui-export.sav'))
    subprocess.run(['dotnet', 'run', '--project', str(root / 'Android/EditorProbe'), '-c', 'Release',
                    '--', '--verify-party-export', str(output / 'ui-export.sav')], check=True)
    (output / 'result.txt').write_text('PASS: Android 16 installation, five cold launches, diagnostic sharing, save import, level edit, held item, party addition/removal, move edit and validated two-member party export.\n')
    print((output / 'result.txt').read_text())
