using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;

public record GameWindowHandlers(
    Action<SKCanvas, SKImageInfo>? Draw = null,
    Action<KeyEventArgs>? KeyDown = null,
    Action<SKPoint>? MouseDown = null,
    Action<SKPoint>? MouseUp = null,
    Func<SKPoint, bool>? MouseMove = null,
    Action<float>? MouseWheel = null
);

public class GameApplication : Application
{
    private GameApplication(GameWindowHandlers handlers)
    {
        this.MainWindow = new GameWindow(handlers);
    }

    public static void Run(GameWindowHandlers handlers)
    {
        // Note: Null checking with !! was dropped for C# 11.
        // Use ArgumentNullException.ThrowIfNull (new in .NET 6) instead.
        // See also https://slides.com/rainerstropek/csharp-11/fullscreen#/4
        ArgumentNullException.ThrowIfNull(handlers);

        Thread.CurrentThread.SetApartmentState(ApartmentState.Unknown);
        Thread.CurrentThread.SetApartmentState(ApartmentState.STA);

        var app = new GameApplication(handlers);
        app.Run();
    }
}

// Note the use of a file-scoped types here (new in C# 11). For details see
// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/file
file static class PointsExtensions
{
    // Did you know that you can add deconstructors to existing types
    // via extension methods? Read more at
    // https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/functional/deconstruct#extension-methods-for-user-defined-types
    public static void Deconstruct(this Point point, out float x, out float y)
    {
        x = (float)point.X;
        y = (float)point.Y;
    }
}

// Note the use of a file-scoped types here (new in C# 11). For details see
// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/file

file class GameWindow : Window
{
    private readonly GameWindowHandlers handlers;
    private DispatcherTimer timer = new();

    public GameWindow(GameWindowHandlers handlers)
    {
        this.handlers = handlers;

        // Create Skia Element on which we will draw
        var element = new SKElement();
        element.PaintSurface += OnPaintSurface;

        // Create main window. Skia element will be the only child.
        var window = new Window() { Content = element };

        // Shutdown app if main window is closed.
        // Note the Lambda discard parameter here. It was added in C# 9.
        // Read more at https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-9.0/lambda-discard-parameters
        window.Closed += (_, _) => Application.Current.Shutdown();

        if (handlers.KeyDown != null)
        {
            window.KeyDown += (_, args) =>
            {
                handlers.KeyDown(args);
                element.InvalidateVisual();
            };
        }
        if (handlers.MouseDown != null)
        {
            window.MouseLeftButtonDown += (_, args) =>
            {
                // Here we use the deconstructor added via extension method
                // (see above in class PointsExtensions).
                var (x, y) = args.GetPosition(element);
                handlers.MouseDown(new(x, y));
                element.InvalidateVisual();
            };
        }
        if (handlers.MouseUp != null)
        {
            window.MouseLeftButtonUp += (_, args) =>
            {
                var (x, y) = args.GetPosition(element);
                handlers.MouseUp(new(x, y));
                element.InvalidateVisual();
            };
        }
        if (handlers.MouseMove != null)
        {
            window.MouseMove += (_, args) =>
            {
                var (x, y) = args.GetPosition(element);
                if (handlers.MouseMove(new(x, y))) { element.InvalidateVisual(); }
            };
        }
        if (handlers.MouseWheel != null)
        {
            window.MouseWheel += (_, args) =>
            {
                handlers.MouseWheel((float)args.Delta);
                element.InvalidateVisual();
            };
        }

        timer.Interval = TimeSpan.FromMilliseconds(1000 / 60);
        timer.Tick += (_, _) => element.InvalidateVisual();
        timer.Start();

        window.Show();
    }

    private void OnPaintSurface(object? _, SKPaintSurfaceEventArgs e)
    {
        handlers.Draw?.Invoke(e.Surface.Canvas, e.Info);
    }
}
