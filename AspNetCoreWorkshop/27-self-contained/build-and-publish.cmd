dotnet restore
dotnet build
dotnet publish -c release
dotnet publish -c release -r win10-x64
dotnet publish -c release -r debian.8-x64