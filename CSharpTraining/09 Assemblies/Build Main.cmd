csc /t:library Person.cs
csc /reference:Person.dll Main.cs
ildasm.exe /OUT=Main.il Main.exe
ildasm.exe /OUT=Main.il Main.exe
ildasm.exe /OUT=Person_DLL.il Person.dll