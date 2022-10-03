using System.Runtime.CompilerServices;
using System.Text;

Console.OutputEncoding = Encoding.Default;

var logger = new Logger();
logger.LogMessage(LogLevel.Error, $"An error. Current date and time: {DateTime.Now}.");
logger.LogMessage(LogLevel.Error, $"It is {true} that an error has occurred.");
logger.LogMessage(LogLevel.Warning, $"It is {false} that everything is ok.");
logger.LogMessage(LogLevel.Error, $"An error. Current time: {new TimeOnly(14, 0, 0)}.");

enum LogLevel { Off, Critical, Error, Warning, Information, Trace }

class Logger
{
    public void LogMessage(LogLevel _, string msg)
    {
        Console.WriteLine(msg);
    }

    public void LogMessage(LogLevel _, LogInterpolatedStringHandler builder)
    {
        Console.WriteLine(builder.GetFormattedText());
    }
}

// To learn more about interplation handlers, see
// https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/interpolated-string-handler

[InterpolatedStringHandler]
ref struct LogInterpolatedStringHandler
{
    readonly StringBuilder builder;

    public LogInterpolatedStringHandler(int literalLength, int formattedCount)
    {
        builder = new StringBuilder(literalLength);
    }

    public void AppendLiteral(string s) => builder.Append(s);

    public void AppendFormatted<T>(T t)
    {
        builder.Append(t switch
        {
            DateTime => "📅 ",
            bool b => b ? "✅ " : "❌ ",
            TimeOnly => "⏱️ ",
            _ => string.Empty
        });
        builder.Append(t?.ToString());
    }

    internal string GetFormattedText() => builder.ToString();
}
