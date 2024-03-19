# Storyboard

## GitHub Copilot CLI

> gh copilot explain "How can I create a C# console app (called TrainStation) with xUnit tests (called TrainStation.Tests) and a solution (called TrainStation) in the console?"

```bash
dotnet new console -n TrainStation
dotnet new xunit -n TrainStation.Tests
dotnet new sln -n TrainStation
dotnet sln TrainStation.sln add TrainStation/TrainStation.csproj
dotnet sln TrainStation.sln add TrainStation.Tests/TrainStation.Tests.csproj
dotnet add TrainStation.Tests/TrainStation.Tests.csproj reference TrainStation/TrainStation.csproj
```

I am putting together an exercise for students learning C#. The goal of the exercise is to practice dynamic data structures, enums, and exceptions. I was thinking of modelling a simple train station logic (switching of wagons and locomotives between tracks). Good idea?

I am thinking of classes Wagon, Track, and TrainStation. Maybe some tracks can be accessed from east and west, some only from west. Which methods would you add to the Track class?

Is it possible to simplify the methods by introducing helper methods? I want to be more DRY.

Generate xUnit unit tests for Track.AddWagon. Make sure to include tests for all exception cases, too.
Generate xUnit unit tests for Track.RemoveWagon. Make sure to include tests for all exception cases, too.
Generate xUnit unit tests for Track.Leave. Make sure to include tests for all exception cases, too.

Generate xUnit unit tests for class TrainStation. Make sure to include tests for all exception cases, too.

I need to put together a description of the exercise. Look at #file:Track.cs and write a chapter for a readme.md file. The target audience is computer science students learning C#. They will not get the ready-made code. They will just get the text that you write and they will have to create the code based on your description.

Look at the classes in #file:Track.cs, #file:Wagon.cs, and #file:TrainStation.cs. Create a mermaid.js class diagram from them. Do not include enums in the class diagram.
