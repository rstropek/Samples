@echo off

REM Generate a console app called "helloWorld"
mkdir helloworld
cd helloworld
dotnet new console

REM Generate a library called "superutils"
REM Goal: Compare .csproj from console app and library
cd ..
mkdir superutils
cd superutils
dotnet new classlib
cd ..

REM Have fun with dotnet CLI
cd helloworld

REM Restore dependencies
dotnet restore
REM Look at what has changed in C:\Users\<user>\.nuget\packages

REM Build app
dotnet build

REM Run app
dotnet run
dotnet bin\Debug\netcoreapp2.2\helloworld.dll

REM Publish app (look at what happens in the file system)
dotnet publish -o out
REM Run the app from the published directory

REM Generate platform-dependent executable
REM Note that the behavior regarding executables will change significantly in
REM .NET Core 3 (see also https://docs.microsoft.com/en-us/dotnet/core/whats-new/dotnet-core-3-0#default-executables).
dotnet publish -c Release -r win-x64 -o out
REM Run the app from published directory
