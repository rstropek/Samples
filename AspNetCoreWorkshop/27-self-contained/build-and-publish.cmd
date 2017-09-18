dotnet restore
dotnet build
dotnet publish -c release
dotnet publish -c release -r win-x64
dotnet publish -c release -r linux-x64