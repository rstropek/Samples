# Isometric game

## Game setup

* Create console app: `dotnet new console`
* Copy spritesheet ([Images/Spritesheet.png](https://github.com/rstropek/Samples/blob/master/CSharp11/IsometricGame/Images/Spritesheet.png)) into *Images* folder.
* Configure project (WPF, reference Skia, include image resources) by copying *.csproj* from [WpfTopLevel.csproj](https://github.com/rstropek/Samples/blob/master/CSharp11/IsometricGame/WpfTopLevel.csproj)

## WPF

* Implement [GameWindow.cs](https://github.com/rstropek/Samples/blob/master/CSharp11/IsometricGame/GameWindow.cs)
  * Handler record
  * Game application
  * Game window (file-scoped types)
* Add handler to *Program.cs*

    ```cs
    using System.Globalization;
    using System.Windows.Input;
    using SkiaSharp;

    void Draw(SKCanvas canvas, SKImageInfo imageInfo)
    {
    }

    void KeyDown(KeyEventArgs key)
    {
    }

    void MouseWheel(float delta) { };
    void MouseDown(SKPoint position) { };
    void MouseUp(SKPoint position) { };
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

* Quickly introduce [Sprite Sheet Packer](https://www.codeandweb.com/free-sprite-sheet-packer)
  * [`FramesJson`](./SpritesheetData.cs)
* Implement [Spritesheet.cs](https://github.com/rstropek/Samples/blob/master/CSharp11/IsometricGame/Spritesheet.cs)
  * Frame and Spritesheet records
  * Add converters, speak about files-scoped types
  * Add Spritesheet loader
  * Add [*SkiaExtensions.cs*](https://github.com/rstropek/Samples/blob/master/CSharp11/IsometricGame/SkiaExtensions.cs) (will not be covered in details)
  * Add [*Player.cs*](https://github.com/rstropek/Samples/blob/master/CSharp11/IsometricGame/Player.cs) (will not be covered in details)
* Add some code snippets to Program:
  * [Constants](https://github.com/rstropek/Samples/blob/d66eb4c29e5ccd1b04247e7a88969de68f2480d4/CSharp11/IsometricGame/Program.cs#L4440)
  * Region *Setup* without Tiles
  * Region *Handler methods* without *Player drawing*
  * Region *Player drawing*

  ```cs
  using System.Globalization;
  using System.Windows.Input;
  using SkiaSharp;

  const string REFERENCE_SPRITE_NAME = "Snow (29).png";
  const float TILE_DISTANCE_X = 70f;
  const float TILE_DISTANCE_Y = 10f;
  const float INITIAL_SCALE = 0.5f;
  const int SIDE_LENGTH = 10;
  const int NUMBER_OF_PLAYER_TILES = 7;
  const float MOUSE_WHEEL_SENSITIVITY = 2000f;

  // Load spritesheet from resources
  var spritesheetData = new SpritesheetData();
  var sprites = SpritesheetLoader.Load(spritesheetData.FramesJson, "Images/Spritesheet.png");

  // Get a reference frame. This reference tile image is used to determine the
  // size of a single tile. All other tile images will be resized to the size
  // of the reference tile.
  var referenceFrame = sprites.Frames[REFERENCE_SPRITE_NAME];
  var tileSize = referenceFrame.SourceSize + new SKSize(TILE_DISTANCE_X, TILE_DISTANCE_Y);

  float scale = INITIAL_SCALE; // Current scale factor (can be changed with mouse wheel)
  SKPoint translation = new SKPoint(); // Current position of the viewport (can be changed with mouse drag)
  SKPoint? mouseDownPosition = null; // Position of the mouse when the mouse button is pressed, for dragging the viewport

  // Create the player sprite. We have multiple sprites (see NUMBER_OF_PLAYER_TILES).
  // The player color changes whenever it picks up a bonus chip.
  var playerTileId = 0;
  var player = new Player(sprites, $"User0{playerTileId + 1}.png", -tileSize.Height / 2.5f);

  void Draw(SKCanvas canvas, SKImageInfo imageInfo)
  {
      // White background
      canvas.Clear(SKColors.White);

      canvas.Save();

      // Initialize viewport if it has not been initialized yet
      if (translation == SKPoint.Empty)
      {
          // Center the game board horizontally, and move it down (from very top) a bit
          translation = new(imageInfo.Width / 2, tileSize.Height * scale);
      }

      // Apply viewport transformation
      canvas.Translate(translation);
      canvas.Scale(scale);

      // Draw the game board
      for (SKPointI pos = new SKPointI(); pos.Y < SIDE_LENGTH; pos.Offset(-pos.X, 1))
      {
          for (; pos.X < SIDE_LENGTH; pos.Offset(1, 0))
          {
              canvas.Save();

              // Move to the tile position
              canvas.Translate(((pos.X - pos.Y) * tileSize.Width) / 2, ((pos.X + pos.Y) * tileSize.Height) / 2);

              #region Player drawing
              // Draw the player if it is on the current position
              if (pos == player.PlayerPosition)
              {
                  // Draw the player
                  player.Draw(canvas);
              }
              #endregion

              canvas.Restore();
          }
      }

      canvas.Restore();
  }

  #region Keyboard handling
  void KeyDown(KeyEventArgs key)
  {
      // We only care about the arrow keys
      if (key.Key is not Key.Up and not Key.Down and not Key.Left and not Key.Right) { return; }

      // Calculate next position
      var nextPosition = player.PlayerPosition;
      // Note that we use the new keyword without a data type here.
      // This is possible because the data type (SKPointI) is inferred 
      // from the left-hand side. With this, we do not need to write
      // the data type SKPointI in each case of the switch expression.
      SKPointI offset = key.Key switch
      {
          Key.Up => new(0, -1),
          Key.Down => new(0, 1),
          Key.Left => new(-1, 0),
          Key.Right => new(1, 0),
          _ => SKPointI.Empty
      };
      nextPosition.Offset(offset);

      if (nextPosition.X >= 0 && nextPosition.Y >= 0 && nextPosition.X < SIDE_LENGTH
          && nextPosition.Y < SIDE_LENGTH)
      { 
          player.PlayerPosition = nextPosition;
      }
  }
  #endregion

  #region Mouse handling
  void MouseWheel(float delta) => scale += delta / MOUSE_WHEEL_SENSITIVITY;
  void MouseDown(SKPoint position) => mouseDownPosition = position;
  void MouseUp(SKPoint position) => mouseDownPosition = null;
  bool MouseMove(SKPoint position)
  {
      if (mouseDownPosition.HasValue)
      {
          translation += position - mouseDownPosition.Value;
          mouseDownPosition = position;
          return true;
      }

      return false;
  }
  #endregion

  GameApplication.Run(new(
      Draw: Draw,
      KeyDown: KeyDown,
      MouseDown: MouseDown,
      MouseUp: MouseUp,
      MouseMove: MouseMove,
      MouseWheel: MouseWheel
  ));
  ```

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
