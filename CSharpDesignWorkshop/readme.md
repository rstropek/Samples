# Design Concepts

## Structures and Value Types

* [*Point*](Polygon.Core/Point.cs)
* [C# design guidelines for structs](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/struct)
* C# 7 News:
  * [`in` parameter modifier](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/in-parameter-modifier)
  * [`ref` returns and locals](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/ref-returns)
  * [`Span` and `Memory`](https://msdn.microsoft.com/en-us/magazine/mt814808.aspx)
  * [`stackalloc`](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/stackalloc)
    * For example see [*TestSutherlandHodgeman*](Polygon.Core.Tests/TestSutherlandHodgeman.cs) and [*TestRayCasting*](Polygon.Core.Tests/TestRayCasting.cs)

## XML Code Docs

* [Docs](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/xmldoc/xml-documentation-comments)
* Tools
  * [GhostDoc](https://submain.com/products/ghostdoc.aspx)
  * [DocFX](https://dotnet.github.io/docfx/index.html)
* Examples
  * [*Point*](Polygon.Core/Point.cs)
  * [Tutorial on GitHub](https://github.com/rstropek/docfx-intro-demo/tree/master/docs/CalculatorDocumentation/articles)
* Tip: [`inheritdoc`](https://dotnet.github.io/docfx/spec/triple_slash_comments_spec.html?q=inheritdoc#inheritdoc)
  * [*RandomPolygonGenerator*](Polygon.Core/RandomPolygonGenerator.cs)

## Immutables

* An *immutable object* is an object whose state cannot be modified after it is created
  * Compare with [*freezables*](https://docs.microsoft.com/en-us/dotnet/framework/wpf/advanced/freezable-objects-overview)
* [*Point*](Polygon.Core/Point.cs)
* Related topics:
  * [Read-only properties](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/how-to-declare-and-use-read-write-properties#robust-programming)

## Language Features

* [Expression-bodied members](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/expression-bodied-members)
* Interfaces vs. abstract base classes
* [Tuples](https://docs.microsoft.com/en-us/dotnet/csharp/tuples)
  * For example see [*MonteCarloAreaCalculator*](Polygon.Core/MonteCarloAreaCalculator.cs)
* [Tuple deconstruction](https://docs.microsoft.com/en-us/dotnet/csharp/deconstruct)
  * For example see [*MonteCarloAreaCalculator*](Polygon.Core/MonteCarloAreaCalculator.cs)
  * For example see [*Point*](Polygon.Core/Point.cs)
* [Examples for `InternalsVisibleTo`](Polygon.Core/AssemblyProperties.cs)
  * For example see [*DoubleExtensions*](Polygon.Core/DoubleExtensions.cs)
* [`nameof`](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/nameof)
  * For example see [*MonteCarloAreaCalculator*](Polygon.Core/MonteCarloAreaCalculator.cs)
* [Pattern matching with `is`](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/is)
  * For example see [*PointsToPointMarkupConverter*](PolygonDesigner.ViewLogic/PointsToPointMarkupConverter.cs)

## Testing

* [*TestPoint*](Polygon.Core.Tests/TestPoint.cs)
* [Live Unit Testing (Enterprise Ed.)](https://docs.microsoft.com/en-us/visualstudio/test/live-unit-testing?view=vs-2017)
* Async testing
  * For example see [*TestMonteCarloAreaCalculator*](Polygon.Core.Tests/TestMonteCarloAreaCalculator.cs)
* [Moq](https://github.com/moq/moq4)
  * For example see [*TestMonteCarloAreaCalculator*](Polygon.Core.Tests/TestMonteCarloAreaCalculator.cs)

## Async Programming

* `Task`
* Cancellation of tasks with `CancellationToken`
  * For example see [*MonteCarloAreaCalculator*](Polygon.Core/MonteCarloAreaCalculator.cs)
* Progress reporting with `IProgress` and `Progress`
  * For example see [*MonteCarloAreaCalculator*](Polygon.Core/MonteCarloAreaCalculator.cs)

## MVVM

* [*Prism* Library](http://prismlibrary.github.io/index.html)
  * Tip: [Prism Template Pack](https://marketplace.visualstudio.com/items?itemName=BrianLagunas.PrismTemplatePack)
  * [Implementing MVVM](http://prismlibrary.github.io/docs/wpf/Implementing-MVVM.html)
    * `BindableBase`

## Various Topics

* [Exception Design](https://docs.microsoft.com/en-us/dotnet/standard/exceptions/best-practices-for-exceptions)
  * For example see [*MonteCarloAreaCalculator*](Polygon.Core/MonteCarloAreaCalculator.cs)
* Partial classes
  * For example see [*MonteCarloAreaCalculator*](Polygon.Core/CalculationController.cs)
* Enabling C# 7.x language features
