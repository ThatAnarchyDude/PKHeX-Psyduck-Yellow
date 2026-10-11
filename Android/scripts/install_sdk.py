#!/usr/bin/env python3
"""Install pinned official Google Android SDK/JDK archives with published hashes."""
from pathlib import Path
import hashlib
import shutil
import tarfile
import urllib.request
import zipfile

root = Path(__file__).resolve().parents[2] / '.tools'
root.mkdir(exist_ok=True)
packages = [
 ('commandlinetools-linux-16111833_latest.zip', 'e025545c62a8e64c7559119566a569fb1dec5f60', 'cmdline-tools/latest'),
 ('platform-tools_r37.0.1-linux.zip', '477254aa5f903c15cf51001717bdf347fb6b53e0', 'platform-tools'),
 ('build-tools_r36_linux.zip', 'b0b6376977657e8ad9b969bacf4093601da2c6fb', 'build-tools/36.0.0'),
 ('platform-36_r02.zip', '2c1a80dd4d9f7d0e6dd336ec603d9b5c55a6f576', 'platforms/android-36'),
]
# SHA-1 values are Google's published SDK repository metadata checksums.
for name, expected, location in packages:
    destination = root / 'android-sdk' / location
    if destination.exists():
        continue
    archive = root / name
    urllib.request.urlretrieve('https://dl.google.com/android/repository/' + name, archive)
    if hashlib.sha1(archive.read_bytes()).hexdigest() != expected:
        raise SystemExit('SDK checksum mismatch: ' + name)
    unpack = root / (name + '-unpack')
    with zipfile.ZipFile(archive) as z:
        z.extractall(unpack)
        for info in z.infolist():
            file = unpack / info.filename
            mode = info.external_attr >> 16
            if mode and file.exists():
                file.chmod(mode)
    children = list(unpack.iterdir())
    if len(children) != 1:
        raise SystemExit('Unexpected SDK archive layout: ' + name)
    destination.parent.mkdir(parents=True, exist_ok=True)
    shutil.move(str(children[0]), destination)
    shutil.rmtree(unpack)
    archive.unlink()
    print('Verified SDK package:', location)
name = 'OpenJDK21U-jdk_x64_linux_hotspot_21.0.12.1_1.tar.gz'
jdk = root / 'jdk/jdk-21.0.12.1+1'
if not jdk.exists():
    archive = root / name
    urllib.request.urlretrieve('https://github.com/adoptium/temurin21-binaries/releases/download/jdk-21.0.12.1%2B1/' + name, archive)
    if hashlib.sha256(archive.read_bytes()).hexdigest() != 'ce79869e1307ed8ee1e2baa86a412b1eb5b75d10a01006d788a6f968bcfaee94':
        raise SystemExit('Official Temurin JDK checksum mismatch.')
    (root / 'jdk').mkdir(exist_ok=True)
    with tarfile.open(archive) as t:
        t.extractall(root / 'jdk', filter='data')
    archive.unlink()
    print('Verified full JDK 21.')
print('SDK:', root / 'android-sdk')
print('JDK:', jdk)
