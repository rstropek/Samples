using Snake.Game;

const int LEFT_MARGIN = 5;
const int TOP_MARGIN = 5;
const int WIDTH = 30;
const int HEIGHT = 10;

var cts = new CancellationTokenSource();
var game = new Game(WIDTH, HEIGHT, 3);
var speed = 500;
var apples = 0;
game.OnFoundApple += (_, _) =>
{
    speed = Math.Max(speed - 25, 10);
    apples++;
};
game.OnDied += (_, _) => cts.Cancel();

var renderer = new GameUI();
var keyboardTask = renderer.StartKeyboardListener(cts.Token);

renderer.KeyPressed += (_, key) =>
{
    if (key == ConsoleKey.Escape)
    {
        cts.Cancel();
        return;
    }

    Direction? newSnakeDirection = key switch
    {
        ConsoleKey.LeftArrow => Direction.Left,
        ConsoleKey.RightArrow => Direction.Right,
        ConsoleKey.UpArrow => Direction.Up,
        ConsoleKey.DownArrow => Direction.Down,
        _ => null
    };

    if (newSnakeDirection is not null)
    {
        game.TrySetSnakeDirection(newSnakeDirection.Value);
    }
};
    
Console.CursorVisible = false;
while (!cts.IsCancellationRequested)
{
    renderer.ClearScreen();
    renderer.DrawRectangle(LEFT_MARGIN - 1, TOP_MARGIN - 1, WIDTH + 2, HEIGHT + 2);
    renderer.Render(game, LEFT_MARGIN, TOP_MARGIN);
    renderer.DrawText(LEFT_MARGIN + WIDTH + LEFT_MARGIN, TOP_MARGIN, $"Points: {apples}");
    try
    {
        await Task.Delay(speed, cts.Token);
    }
    catch (TaskCanceledException)
    {
        break;
    }

    game.Move();
}

await keyboardTask;
