using ConsoleTetris;
using Microsoft.Extensions.DependencyInjection;

const int LEFT = 6;
const int TOP = 2;
const int WIDTH = 6;
const int HEIGHT = 10;

var services = new ServiceCollection();
services.AddSingleton<ITetrisConsole, TetrisConsole>();
services.AddSingleton<TetrisBlock>();
services.AddSingleton<AsciiDrawing>();
var serviceProvider = services.BuildServiceProvider();
var asciiDrawing = serviceProvider.GetRequiredService<AsciiDrawing>();
var blocks = serviceProvider.GetRequiredService<TetrisBlock>();
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
var lines = 0;
object drawingLock = new();

bool NewBlock()
{
    block = blocks.GetRandomBlock();
    x = WIDTH / 2;
    y = 0;
    return canvas.Fits(x, y, block);
}

void Redraw()
{
    lock (drawingLock)
    {
        asciiDrawing.ClearRectangle(WIDTH, HEIGHT);
        canvas.Draw();
        asciiDrawing.DrawBlock(x, y, block);
        console.SetCursorPosition(WIDTH + 5, 1);
        console.Write($"Removed lines: {lines}");

    }
}

NewBlock();
var cts = new CancellationTokenSource();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
console.ListenForKey([ConsoleKey.LeftArrow, ConsoleKey.RightArrow, ConsoleKey.DownArrow, ConsoleKey.Escape, ConsoleKey.Spacebar], key =>
{
    int originalX = x, originalY = y;
    var originalBlock = block;
    switch (key.Key)
    {
        case ConsoleKey.LeftArrow:
            if (canvas.Fits(x - 1, y, block))
            {
                x--;
            }
            break;
        case ConsoleKey.RightArrow:
            if (canvas.Fits(x + 1, y, block))
            {
                x++;
            }
            break;
        case ConsoleKey.Escape:
            cts.Cancel();
            quit = true;
            break;
        case ConsoleKey.Spacebar:
            var rotated = blocks.Rotate(block);
            if (canvas.Fits(x, y, rotated))
            {
                block = rotated;
            }
            break;
        case ConsoleKey.DownArrow:
            y = canvas.Drop(x, y, block);
            break;
        default:
            break;
    }

    if (x != originalX || y != originalY || block != originalBlock)
    {
        Redraw();
    }
}, cts.Token);
#pragma warning restore CS4014

try
{
    while (!quit)
    {
        Redraw();
        await Task.Delay(TimeSpan.FromSeconds(1), cts.Token);
        if (!canvas.Fits(x, y + 1, block))
        {
            canvas.SetPixel(x, y, block);
            lines += canvas.Shift();
            if (!NewBlock())
            {
                Redraw();
                quit = true;
            }
        }
        else
        {
            y++;
        }
    }
}
catch (OperationCanceledException) { }

Console.CursorVisible = true;
