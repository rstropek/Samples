namespace ConsoleTetris;

public interface ITetrisConsole
{
    void WriteLine(string line);
    void Write(string line);
    void Write(char c);
    void SetCursorPosition(int left, int top);
    void Clear();
    int TranslateX { get; set; }
    int TranslateY { get; set; }
    Task ListenForKey(IEnumerable<ConsoleKey> keysToListenFor, Action<ConsoleKeyInfo> callback, CancellationToken cancellationToken);
    ConsoleColor BackgroundColor { get; set; }
    ConsoleColor ForegroundColor { get; set; }
}

/// <summary>
/// Wrapper around the Console class for mocking
/// </summary>
public class TetrisConsole : ITetrisConsole
{
    public void WriteLine(string line) => Console.WriteLine(line);
    public void Write(string line) => Console.Write(line);
    public void Write(char c) => Console.Write(c);
    public void SetCursorPosition(int left, int top) => Console.SetCursorPosition(TranslateX + left, TranslateY + top);
    public void Clear() => Console.Clear();
    public ConsoleColor BackgroundColor
    {
        get => Console.BackgroundColor;
        set => Console.BackgroundColor = value;
    }
    public ConsoleColor ForegroundColor
    {
        get => Console.ForegroundColor;
        set => Console.ForegroundColor = value;
    }
    public int TranslateX { get; set; } = 0;
    public int TranslateY { get; set; } = 0;
    public async Task ListenForKey(IEnumerable<ConsoleKey> keysToListenFor, Action<ConsoleKeyInfo> callback, CancellationToken cancellationToken)
    {
        var keysSet = new HashSet<ConsoleKey>(keysToListenFor);
        while (!cancellationToken.IsCancellationRequested)
        {
            if (Console.KeyAvailable)
            {
                var keyInfo = Console.ReadKey(true);
                if (keysSet.Contains(keyInfo.Key))
                {
                    callback(keyInfo);
                }
            }
            await Task.Delay(10, cancellationToken); // Short delay to prevent busy waiting
        }
    }
}
