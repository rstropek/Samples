#!/usr/bin/env bash
# Reproduce the IL / lowering shown in docs/01-CSharp15-Unions.md §3.
# Requires: .NET 11 SDK + `dotnet tool install -g ilspycmd`.
set -euo pipefail
cd "$(dirname "$0")"

dotnet build -c Release
DLL="bin/Release/net11.0/06_Lowering.dll"

echo "===================  Decompiled C# (the lowering)  ==================="
ilspycmd "$DLL" -t Lowering.Shape

echo
echo "===================  IL of the exhaustive switch  ===================="
ilspycmd "$DLL" -il | awk '/float64 Area \(/,/end of method Consumer::Area/'
