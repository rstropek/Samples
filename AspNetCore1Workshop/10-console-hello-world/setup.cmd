REM *** Prerequisite: 
REM     Node.js (http://nodejs.org)
REM     ASP.NET 5 (https://docs.asp.net/en/latest/getting-started/index.html)

REM Install yeoman (http://yeoman.io/)
npm install -g yo

REM Install generator for ASP.NET (https://www.npmjs.com/package/generator-aspnet)
npm install -g generator-aspnet

REM Generate a console app called "helloWorld"
yo aspnet console helloWorld
cd helloWorld

REM Have fun with DNVM
REM List available versions
dnvm list
REM Install a version
dnvm install latest -r coreclr -arch x64
REM Use a version (note change to PATH when persisting with -p) 
dnvm use 1.0.0-rc1-update1 -r coreclr -arch x64
dnvm use 1.0.0-rc1-update1 -r coreclr -arch x64 -p
REM Note where .NET tools come from
where dnu

REM Add an alias to type less
dnvm alias default 1.0.0-rc1-update1 -r coreclr -arch x64
REM Look at what has changed in C:\Users\<user>\.dnx
dnvm use default -p

REM Restore dependencies
dnu restore
REM Look at what has changed in C:\Users\<user>\.dnx

REM Run app
dnx run

REM Run app with tracing
set DNX_TRACE=1
dnx run

REM Build and publish app (look at what happens in the file system)
dnu build
dnu publish
REM Run the app from the published directory
