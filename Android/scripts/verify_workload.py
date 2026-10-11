#!/usr/bin/env python3
"""Restore explicit signature verification after .NET's workload installation."""
import os
from pathlib import Path
import subprocess
import shutil

dotnet = shutil.which('dotnet')
if not dotnet:
    raise SystemExit('Put the .NET 10 SDK on PATH first.')
root = Path(os.environ.get('DOTNET_ROOT', Path(dotnet).resolve().parent))
packages = list((root / 'packs').rglob('*.nupkg'))
if not packages:
    raise SystemExit('No retained workload packages found; cannot verify this installation.')
for package in packages:
    result = subprocess.run([dotnet, 'nuget', 'verify', str(package), '--all'], capture_output=True, text=True)
    if result.returncode:
        print(result.stdout, result.stderr)
        raise SystemExit(result.returncode)
print(f'Verified {len(packages)} workload package signatures.')
