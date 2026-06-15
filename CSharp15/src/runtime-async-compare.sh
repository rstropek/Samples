#!/usr/bin/env bash
# Build both runtime-async sample projects and show the difference in
#   (1) the live stack trace, and (2) the emitted IL (state machine vs AsyncHelpers.Await).
# Requires .NET 11 preview SDK + `dotnet tool install -g ilspycmd`.
set -euo pipefail
cd "$(dirname "$0")"

CLASSIC=08_RuntimeAsync_Classic
ENABLED=09_RuntimeAsync_Enabled

echo "##### CLASSIC (compiler state machine) #####"
dotnet run --project $CLASSIC -c Release

echo
echo "##### RUNTIME-ASYNC (runtime-managed) #####"
dotnet run --project $ENABLED -c Release

echo
echo "##### IL: state-machine types per build #####"
echo -n "classic IAsyncStateMachine impls: "
ilspycmd $CLASSIC/bin/Release/net11.0/$CLASSIC.dll -il | grep -c "implements .*IAsyncStateMachine" || true
echo -n "runtime-async IAsyncStateMachine impls: "
ilspycmd $ENABLED/bin/Release/net11.0/$ENABLED.dll -il | grep -c "implements .*IAsyncStateMachine" || true
echo "runtime-async await lowering:"
ilspycmd $ENABLED/bin/Release/net11.0/$ENABLED.dll -il | grep "AsyncHelpers::Await" | head -1
