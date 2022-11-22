# Isometric game

## Game setup

* Create console app: `dotnet new console`
* Copy spritesheet ([Images/Spritesheet.png](https://github.com/rstropek/Samples/blob/master/CSharp11/IsometricGame/Images/Spritesheet.png)) into *Images* folder.
* Configure project (WPF, reference Skia, include image resources) by copying *.csproj* from [WpfTopLevel.csproj](https://github.com/rstropek/Samples/blob/master/CSharp11/IsometricGame/WpfTopLevel.csproj)
* Add [*SkiaExtensions.cs*](https://github.com/rstropek/Samples/blob/master/CSharp11/IsometricGame/SkiaExtensions.cs) (will not be covered in details)
* Add [*Player.cs*](https://github.com/rstropek/Samples/blob/master/CSharp11/IsometricGame/Player.cs) (will not be covered in details)

## WPF

* Implement [GameWindow.cs](https://github.com/rstropek/Samples/blob/master/CSharp11/IsometricGame/GameWindow.cs)
  * Handler record
  * Game application
  * Game window (file-scoped types)
* Add handler to *Program.cs*

    ```cs
    void Draw(SKCanvas canvas, SKImageInfo imageInfo)
    {
    }

    void KeyDown(KeyEventArgs key)
    {
    }

    void MouseWheel(float delta) => { };
    void MouseDown(SKPoint position) => { };
    void MouseUp(SKPoint position) => { };
    bool MouseMove(SKPoint position)
    {
        return false;
    }

    GameApplication.Run(new(
        Draw: Draw,
        KeyDown: KeyDown,
        MouseDown: MouseDown,
        MouseUp: MouseUp,
        MouseMove: MouseMove,
        MouseWheel: MouseWheel
    ));
    ```

## Spritesheet JSON

* Quickly introduce [Sprite Sheet Packer](://www.codeandweb.com/free-sprite-sheet-packer)
  * [`framesJson`](https://github.com/rstropek/Samples/blob/825fa2543b60e04727265cb528b41dc8bd803391/CSharp11/IsometricGame/Program.cs#L11)
* Implement [Spritesheet.cs](https://github.com/rstropek/Samples/blob/master/CSharp11/IsometricGame/Spritesheet.cs)
  * Frame and Spritesheet records
  * Add converters, speak about files-scoped types
  * Add Spritesheet loader
* Add some code snippets to Program:
  * [Constants](https://github.com/rstropek/Samples/blob/d66eb4c29e5ccd1b04247e7a88969de68f2480d4/CSharp11/IsometricGame/Program.cs#L4440)
  * Region *Setup*
  * Region *Handler methods* without *Player drawing*
  * Region *Player drawing*

## Add Tiles

* Tiles
  * Add TileTypes, TileCategory, and TileArrayExtensions (nothing special here)
  * Add Tile (without *Parsable* region)
  * Add *Parsable* region
* Decoration
  * Add DecorationTypes
  * Add Decoration
* Level building
  * Add region *Level building* without *List patterns* in Program
  * Add region *List patterns*
