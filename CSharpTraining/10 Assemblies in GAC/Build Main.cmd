csc /t:library /keyfile:Key.snk Person.cs
csc /t:exe /reference:Person.dll Main.cs
ildasm.exe /OUT=Main.il Main.exe
ildasm.exe /OUT=Person.il Person.dll
