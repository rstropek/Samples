#!/usr/bin/env bash
# Reproduce the IL / lowering shown in TALK-CONCEPT.md.
# Requires: .NET 11 preview SDK + `dotnet tool install -g ilspycmd`.
set -euo pipefail
cd "$(dirname "$0")"

dotnet build -c Release
DLL="bin/Release/net11.0/06_Lowering.dll"

echo "===================  Decompiled C# (the lowering)  ==================="
ilspycmd "$DLL" -t Lowering.Shape

echo
echo "===================  IL of the exhaustive switch  ===================="
ilspycmd "$DLL" -il | awk '/Consumer::Area/,/end of method Consumer::Area/'
