#!/usr/bin/env python3
"""Build a self-contained signed prototype APK with a retained private dev key."""
from pathlib import Path
import os
import shutil
import subprocess
import xml.etree.ElementTree as ET

root = Path(__file__).resolve().parents[2]
sdk = os.environ.get('ANDROID_SDK_ROOT') or os.environ.get('ANDROID_HOME')
java = os.environ.get('JAVA_HOME')
if not sdk or not java or not (Path(java) / 'bin/jar').exists():
    raise SystemExit('Set ANDROID_SDK_ROOT and JAVA_HOME (a full JDK 21, not a JRE).')
key = root / '.tools/signing/editor-dev.jks'
key.parent.mkdir(parents=True, exist_ok=True)
if not key.exists():
    subprocess.run([str(Path(java) / 'bin/keytool'), '-genkeypair', '-keystore', str(key),
                    '-alias', 'psyduck', '-storepass', 'android', '-keypass', 'android',
                    '-dname', 'CN=Psyduck Yellow Experimental Editor', '-keyalg', 'RSA',
                    '-keysize', '2048', '-validity', '10000'], check=True)
subprocess.run(['dotnet', 'build', str(root / 'Android/PsyduckSaveEditor/PsyduckSaveEditor.csproj'),
                '-c', 'Release', f'-p:AndroidSdkDirectory={sdk}', f'-p:JavaSdkDirectory={java}',
                '-p:AndroidKeyStore=true', f'-p:AndroidSigningKeyStore={key}',
                '-p:AndroidSigningKeyAlias=psyduck', '-p:AndroidSigningStorePass=android',
                '-p:AndroidSigningKeyPass=android'], cwd=root, check=True)
apk = root / 'Android/PsyduckSaveEditor/bin/Release/net10.0-android/org.psyduckyellow.saveeditor-Signed.apk'
if not apk.exists():
    raise SystemExit('Build succeeded but expected signed APK is missing.')
version = ET.parse(root / 'Android/PsyduckSaveEditor/PsyduckSaveEditor.csproj').findtext('.//ApplicationDisplayVersion')
output = root / f'Android/artifacts/Psyduck-Save-Editor-{version}.apk'
output.parent.mkdir(parents=True, exist_ok=True)
shutil.copy2(apk, output)
subprocess.run([str(Path(sdk) / 'build-tools/36.0.0/apksigner'), 'verify', '--verbose', str(output)], check=True)
print(f'APK: {output}')
