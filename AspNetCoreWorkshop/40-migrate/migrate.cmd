dotnet migrate --skip-backup
dotnet new sln -n library
dotnet sln library.sln add lib\lib.csproj
dotnet sln library.sln add consumer\consumer.csproj
erase lib\project.json
erase consumer\project.json
