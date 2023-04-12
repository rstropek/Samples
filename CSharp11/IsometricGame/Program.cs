using System.Globalization;
using System.Windows.Input;
using SkiaSharp;

#region Setup
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

// Create the game board
//var level = new string[SIDE_LENGTH][];
var level = new Tile[SIDE_LENGTH][];
#endregion

#region Level building
/*
// Simple level
for (int y = 0; y < SIDE_LENGTH; y++)
{
    level[y] = new string[SIDE_LENGTH];
    for (int x = 0; x < SIDE_LENGTH; x++)
    {
        level[y][x] = "Arctic (15).png";
    }
}
*/
// Current theme
var theme = TileTypes.Arctic;

// Generate 10_000 tile rows. This is for demo purposes only!
// Obviously this would not make sense in a real-world application. We generate
// many random rows to be able to demonstrate different variants of list patterns.
// You will see that a few lines below where we build the level.
var randomRows = Enumerable.Range(0, 10_000)
    .Select(_ => Enumerable.Range(0, SIDE_LENGTH).Select(_ => TileTypes.GetRandom(theme)).ToArray())
    .ToArray();

// Generic parse method. It works with any parsable type, including Tile.
// It splits a string by blanks and parses each resulting part.
// This approach is for demo purposes only. It is used to demonstrate the
// new generic parsing feature of .NET.
static T[] Parse<T>(string rowString) where T : IParsable<T>
    => rowString.Trim().Split(' ').Select(item => T.Parse(item, CultureInfo.InvariantCulture)).ToArray();

var row = 0;
// Demonstrate generic parse method to add a hand-crafted row to our level
// See also https://slides.com/rainerstropek/csharp-11/fullscreen#/11
level[row++] = Parse<Tile>("A2 A2 A3 A4 A4 A7 A14 A14 A15 A2");

#region List patterns
// Select random rows using list patterns.
// See also https://slides.com/rainerstropek/csharp-11/fullscreen#/4
level[row++] = randomRows.First(r => r is [{ IsEmpty: false, Decoration: null }, ..] && !r.Any(r => r.IsBridge)).CloneTiles();
level[row++] = randomRows.First(r => r is [{ IsBridge: false }, { Type: "" }, .., { IsBridge: false }]).CloneTiles();
level[row++] = randomRows.First(r => r is [{ IsBridge: false }, { IsBridge: true }, { IsBridge: false }, .., { IsBridge: false }]).CloneTiles();
level[row++] = randomRows.First(r => r is [{ IsBridge: false }, { Type: "" }, .., { IsBridge: false }]).CloneTiles();
level[row++] = randomRows.First(r => !r.Any(r => r.IsBridge)).CloneTiles();
#endregion

// Fill up the remaining rows with random tiles
for (; row < SIDE_LENGTH; row++) { level[row] = randomRows[Random.Shared.Next(0, randomRows.Length)].CloneTiles(); }
#endregion

#region Handler methods
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

            // Draw the tile
            var tile = level[pos.Y][pos.X];
            //canvas.DrawCenteredBitmap(sprites, tile, referenceFrame.SourceSize);
            tile.Draw(sprites, canvas, referenceFrame.SourceSize);

            #region Player drawing
            // Draw the player if it is on the current position
            if (pos == player.PlayerPosition)
            {
                // Check if player picked up a bonus chip
                if (tile.Decoration?.Pickable ?? false)
                {
                    // Remove bonus chip
                    tile.Decoration = null;

                    // Change player color
                    playerTileId = (playerTileId + 1) % NUMBER_OF_PLAYER_TILES;
                    player.SpriteName = $"User0{playerTileId + 1}.png";
                }

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
    var offset = key.Key switch
    {
        Key.Up => new SKPointI(0, -1),
        Key.Down => new SKPointI(0, 1),
        Key.Left => new SKPointI(-1, 0),
        Key.Right => new SKPointI(1, 0),
        _ => SKPointI.Empty
    };
    nextPosition.Offset(offset);

    if (nextPosition.X >= 0 && nextPosition.Y >= 0 && nextPosition.X < SIDE_LENGTH
        && nextPosition.Y < SIDE_LENGTH
        && level[nextPosition.Y][nextPosition.X].CanGoTo)
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
#endregion
