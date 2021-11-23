# Storyboard

## Getting Started

* Create new console app with .NET 6
* Discuss project settings
* Change project settings:
  * `<EnablePreviewFeatures>True</EnablePreviewFeatures>`
  * `<AllowUnsafeBlocks>true</AllowUnsafeBlocks>`
* Add *Skia*:

    ```xml
    <ItemGroup>
        <PackageReference Include="SkiaSharp" Version="2.88.0-preview.155" />
    </ItemGroup>
    ```

* Add *Usings.cs* with global usings
  * Discuss global usings

## String Interpolations and Linq Enhancements

* Add code for generating seeded random number generator (region *Generate hash based on user name*)
* Show generated IL Code for string interpolation with *dnSpy*
* Point out Linq enhancements
  * https://slides.com/rainerstropek/dotnet-6/fullscreen#/7
  * https://github.com/rstropek/Samples/blob/master/DotNet6/LinqEnhancements/Program.cs

## Record Structs

* Add code for lines and vectors (region *Line and Vector*)
* Discuss record structs
  * https://slides.com/rainerstropek/csharp-10-bettercode/fullscreen#/2
  * https://github.com/rstropek/Samples/blob/master/DotNet6/RecordStruct/Program.cs

## Caller Argument Expressions

* Add code for allocating memory for tree on stack
  * Discuss stack allocation
  * `Span<Line> lines = stackalloc Line[numberOfLines]; // Note: Stack allocation of record struct`
* Add memory limiting mechanism (region *Allocate memory for tree*)
  * Discuss caller argument expression by enforcing exception

## Lambda Improvements and Record Struct Handling

* Add code for generating branch lines (region *Generate tree geometry*)
  * Without lines aggregation

## Create Skia Drawing Canvas

* Region *Create Skia drawing canvas*
  * We will use it later for demonstrating bounding rect usage
* Add header text (region *Draw header text*)
  * Without total length
  * Point out interpolated string handling again
* Save PNG and open it (region *Save PNG and open it*)

## Record Struct Hash Code

* Add code for drawing branches (region *Draw branches*)
  * First without color caching
* Add color caching
  * Show generated `GetHashCode` in *dnSpy*

## Static Abstract Members in Interfaces

* Region *Line aggregations*
  * Add `BoundsRect` without interface
    * Discuss `with` clause for record structs
  * Add `VectorLength` without interface
  * Add `ISupportAdding`, add it to previously added structs
  * Add `SumOfLines`
* Add aggregating lines to *Generate tree geometry* region
* Change `canvas.Translate` in *Create Skia drawing canvas*
* Add bounding rect (region *Draw bounding rectangle*)
* Add total length drawing to region *Draw header text*
  * Discuss interpolated string handling with *dnSpy*
