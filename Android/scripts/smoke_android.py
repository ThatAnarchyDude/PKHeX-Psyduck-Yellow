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

adb('install', '-r', str(root / 'Android/artifacts/Psyduck-Save-Editor-0.1.0.apk'))
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
(output / 'result.txt').write_text('PASS: Android 16 APK installation, app launch, system file picker, cancellation/resume.\n')
print((output / 'result.txt').read_text())
