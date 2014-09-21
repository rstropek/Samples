REM Generate nuget spec
tools\nuget.exe spec

tools\nuget.exe pack PiWithMonteCarlo.csproj
tools\nuget.exe pack PiWithMonteCarlo.csproj -Symbols
