# Test-Driven Development Storyboard

## Project Setup

* Create the following projects:

| Project Name                      | Type                            | Description                                                                         |
| --------------------------------- | ------------------------------- | ----------------------------------------------------------------------------------- |
| *Polygon.Core*                    | .NET Standard 2.1 Class Library | Core business logic for the app                                                     |
| *Polygon.Core.Tests*              | xUnit Tests (.NET Core 3.1)     | Tests for core business logic, references *Polygon.Core*                            |
| *PolygonDesigner.ViewLogic*       | .NET Standard 2.1 Class Library | View logic for UI, references *Polygon.Core*                                        |
| *PolygonDesigner*                 | WPF App (.NET Core 3.1)         | UI for Poloygon Designer, references *Polygon.Core* and *PolygonDesigner.ViewLogic* |
| *PolygonDesigner.ViewLogic.Tests* | xUnit Tests (.NET Core 3.1)     | Tests for UI logic, references *PolygonDesigner.ViewLogic*                          |

Measures to ensure good code quality, ensure the following for *all* project files:

* Enable the use of latest C# features: `<LangVersion>latest</LangVersion>`
* Enable [nullable reference types](https://docs.microsoft.com/en-us/dotnet/csharp/tutorials/nullable-reference-types): `<Nullable>enable</Nullable>`
* Add [*Microsoft.CodeAnalysis.FxCopAnalyzers*](https://docs.microsoft.com/en-us/visualstudio/code-quality/install-fxcop-analyzers) analyzers
* Add [*GlobalSuppressions.cs*](PolygonDesigner/Polygon.Core/GlobalSuppressions.cs) to *Polygon.Core*

In order to make internals visible to test projects, add [*AssemblyProperties.cs*](PolygonDesigner/Polygon.Core/AssemblyProperties.cs) to *Polygon.Core*. Discuss `InternalsVisibleTrue` attribute.

## Basic Business Logic

### Basics

* Add to *Polygon.Core*:
  * [*Point.cs*](PolygonDesigner/Polygon.Core/Point.cs) and 
  * [*Vector.cs*](PolygonDesigner/Polygon.Core/Vector.cs)
* Add to *Polygon.Core.Tests*:
  * [*TestPoint.cs*](PolygonDesigner/Polygon.Core.Tests/TestPoint.cs) and 
  * [*TestVector.cs*](PolygonDesigner/Polygon.Core.Tests/TestVector.cs)
* Discuss testing basics based on the two test files
* Add [*Edge.cs*](PolygonDesigner/Polygon.Core/Edge.cs) to *Polygon.Core*
* Add [*TestEdge.cs*](PolygonDesigner/Polygon.Core.Tests/TestEdge.cs) to *Polygon.Core.Tests*
* Discuss testing of more complex business logic in *Edge.cs*
* Demonstrate tests in *Code Lense*
* Demonstrate live unit testing based on *TestEdge.cs*
* Add [*DoubleExtensions.cs*](PolygonDesigner/Polygon.Core/DoubleExtensions.cs) to *Polygon.Core*
* Add [*TestDoubleExtensions.cs*](PolygonDesigner/Polygon.Core.Tests/TestDoubleExtensions.cs) to *Polygon.Core.Tests*

### Continuous Integration

* Discuss test and coverage results in CI ([example run](https://dev.azure.com/rainerdemotfs-westeu/TDD-Workshop/_build/results?buildId=667&view=results))
* Add [*azure-pipelines.yml*](PolygonDesigner/azure-pipelines.yml) to solution's root folder
  * Option: Create *Azure DevOps* project, create pipeline, run pipeline and explore test results

### Calculation Logic

* Add [generators](PolygonDesigner/Polygon.Core/Generators) to *Polygon.Core*
  * Discuss `IPolygonGenerator` abstraction
* Add [*TestPolygonGenerator.cs*](PolygonDesigner/Polygon.Core.Tests/TestPolygonGenerator.cs) to *Polygon.Core.Tests*
  * Discuss xUnit `Theory` and data-driven tests
* Add to *Polygon.Core*:
  * [*ContainmentChecker.cs*](PolygonDesigner/Polygon.Core/ContainmentChecker.cs)
  * [*RayCasting.cs*](PolygonDesigner/Polygon.Core/RayCasting.cs)
  * Discuss `IContainmentChecker` abstraction
* Add [*TestRayCasting*](PolygonDesigner/Polygon.Core.Tests/TestRayCasting.cs) to *Polygon.Core.Tests*
* Add to *Polygon.Core*:
  * [*AreaCalculator.cs*](PolygonDesigner/Polygon.Core/AreaCalculator.cs)
  * [*CalculationController.cs*](PolygonDesigner/Polygon.Core/CalculationController.cs)
  * [*MonteCarloAreaCalculator.cs*](PolygonDesigner/Polygon.Core/MonteCarloAreaCalculator.cs)
  * [*MonteCarloAreaCalculatorOptions.cs*](PolygonDesigner/Polygon.Core/MonteCarloAreaCalculatorOptions.cs)
* Discuss `IAreaCalculator` abstraction
* Add [*TestMonteCarloAreaCalculator.cs*](PolygonDesigner/Polygon.Core.Tests/TestMonteCarloAreaCalculator.cs) to *Polygon.Core.Tests*
  * Discuss [mocking framework *Moq*](https://github.com/moq/moq4) used in `TestMonteCarloAreaCalculator` class
* Add to *Polygon.Core*:
  * [*IPolygonClipper.cs*](PolygonDesigner/Polygon.Core/IPolygonClipper.cs)
  * [*SutherlandHodgeman.cs*](PolygonDesigner/Polygon.Core/SutherlandHodgeman.cs)
  * Discuss `IPolygonClipper` abstraction
* Add [*TestSutherlandHodgeman.cs*](PolygonDesigner/Polygon.Core.Tests/TestSutherlandHodgeman.cs)

### Benchmarking

* Add [*PointsToPathMarkup.cs*](PolygonDesigner/Polygon.Core/PointsToPathMarkup.cs) to *Polygon.Core*
* Add [*TestPointsToPathMarkup.cs*](PolygonDesigner/Polygon.Core.Tests/TestPointsToPathMarkup.cs) to *Polygon.Core.Tests*
* Add new project called *PointsToMarkupBenchmark* to solution:
  * .NET Core 3.1 Console App
  * References *Polygon.Core*
  * Apply same mechanisms for code quality as mentioned at the beginning
  * Add [*BenchmarkDotNet*](https://benchmarkdotnet.org/index.html) NuGet package
* Add [*Program.cs*](PolygonDesigner/PointsToMarkupBenchmark/Program.cs) go *PointsToMarkupBenchmark*
  * Run benchmarks and discuss benchmark test setup and results
