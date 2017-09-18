@echo off
REM *** Prerequisite: 
REM https://www.microsoft.com/net/core#windows

REM *** For more details see 
REM https://docs.microsoft.com/en-us/dotnet/articles/core/tools/dotnet

REM Generate a console app called "helloWorld"
mkdir helloworld
cd helloworld
dotnet new console

REM Generate a library called "superutils"
REM Goal: Compare project.json from console app and library
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
dotnet bin\Debug\netcoreapp2.0\helloworld.dll

REM Publish app (look at what happens in the file system)
dotnet publish
REM Run the app from the published directory
REM *** For more details see also
REM https://github.com/dotnet/cli/blob/rel/1.0.0/Documentation/specs/runtime-configuration-file.md
