using ConsoleTetris;
using Microsoft.Extensions.DependencyInjection;

const int LEFT = 6;
const int TOP = 2;
const int WIDTH = 6;
const int HEIGHT = 10;

var services = new ServiceCollection();
services.AddSingleton<ITetrisConsole, TetrisConsole>();
services.AddSingleton<Blocks>();
services.AddSingleton<AsciiDrawing>();

var serviceProvider = services.BuildServiceProvider();
var asciiDrawing = serviceProvider.GetRequiredService<AsciiDrawing>();
var blocks = serviceProvider.GetRequiredService<Blocks>();
var console = serviceProvider.GetRequiredService<ITetrisConsole>();
var canvas = new Canvas(WIDTH, HEIGHT, console, blocks);

Console.Clear();
Console.CursorVisible = false;

asciiDrawing.DrawRectangle(LEFT - 1, TOP - 1, WIDTH, HEIGHT);
console.TranslateX = LEFT;
console.TranslateY = TOP;
int x = 0, y = 0;
string block;
var quit = false;

void NewBlock()
{
    block = blocks.GetRandomBlock();
    x = WIDTH / 2;
    y = 0;
}

NewBlock();
var cts = new CancellationTokenSource();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
console.ListenForKey([ConsoleKey.LeftArrow, ConsoleKey.RightArrow, ConsoleKey.DownArrow, ConsoleKey.Escape, ConsoleKey.Spacebar], key =>
{
    int originalX = x, originalY = y;
    switch (key.Key)
    {
        case ConsoleKey.LeftArrow:
            x--;
            break;
        case ConsoleKey.RightArrow:
            x++;
            break;
        case ConsoleKey.Escape:
            cts.Cancel();
            quit = true;
            break;
        case ConsoleKey.Spacebar:
            var rotated = blocks.Rotate(block);
            if (canvas.Fits(x, y, rotated))
            {
                asciiDrawing.DrawBlock(x, y, block, DrawMode.Remove);
                block = rotated;
                asciiDrawing.DrawBlock(x, y, block);
            }
            break;
        case ConsoleKey.DownArrow:
            var newY = canvas.Drop(x, y, block);
            if (newY != y)
            {
                asciiDrawing.DrawBlock(x, y, block, DrawMode.Remove);
                y = newY;
                asciiDrawing.DrawBlock(x, y, block);
            }

            break;
        default:
            break;
    }

    if ((x != originalX || y != originalY) && canvas.Fits(x, y, block))
    {
        asciiDrawing.DrawBlock(originalX, originalY, block, DrawMode.Remove);
        asciiDrawing.DrawBlock(x, y, block);
    }
    else
    {
        x = originalX;
        y = originalY;
    }
}, cts.Token);
#pragma warning restore CS4014

try
{
    while (!quit)
    {
        asciiDrawing.DrawBlock(x, y, block);
        await Task.Delay(TimeSpan.FromSeconds(1), cts.Token);
        if (!canvas.Fits(x, y + 1, block))
        {
            canvas.SetPixel(x, y, block);
            canvas.Shift();
            NewBlock();
        }
        else
        {
            asciiDrawing.DrawBlock(x, y, block, DrawMode.Remove);
            y++;
        }
    }
}
catch (OperationCanceledException) { }

Console.CursorVisible = true;
