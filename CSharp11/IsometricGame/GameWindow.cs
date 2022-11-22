using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;

public record struct GameWindowHandlers(
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
        // See also https://slides.com/rainerstropek/csharp-11/fullscreen#/5
        ArgumentNullException.ThrowIfNull(handlers);

        Thread.CurrentThread.SetApartmentState(ApartmentState.Unknown);
        Thread.CurrentThread.SetApartmentState(ApartmentState.STA);

        var app = new GameApplication(handlers);
        app.Run();
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
        window.Closed += (_, args) => Application.Current.Shutdown();


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
                var pos = args.GetPosition(element);
                handlers.MouseDown(new((float)pos.X, (float)pos.Y));
                element.InvalidateVisual();
            };
        }
        if (handlers.MouseUp != null)
        {
            window.MouseLeftButtonUp += (_, args) =>
            {
                var pos = args.GetPosition(element);
                handlers.MouseUp(new((float)pos.X, (float)pos.Y));
                element.InvalidateVisual();
            };
        }
        if (handlers.MouseMove != null)
        {
            window.MouseMove += (_, args) =>
            {
                var pos = args.GetPosition(element);
                if (handlers.MouseMove(new((float)pos.X, (float)pos.Y)))
                {
                    element.InvalidateVisual();
                }
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