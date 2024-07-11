using SkiaSharp;
using SkiaSharp.Views.WPF;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Shooter;

public partial class App : Application
{
    private readonly Game game = new();
    private readonly DispatcherTimer Timer = new();

    protected override void OnStartup(StartupEventArgs e)
    {
        MainWindow = new Window()
        { 
            Title = "Space Shooter",
            Width = 500,
            Height = 700,
            ResizeMode = ResizeMode.NoResize,
        };
        var keyboard = new KeyboardStatus();
        MainWindow.KeyDown += (s, e) =>
        {
            if (e.Key == Key.A) { keyboard.LeftIsDown = true; }
            if (e.Key == Key.D) { keyboard.RightIsDown = true; }
            if (e.Key == Key.W) { keyboard.UpIsDown = true; }
            if (e.Key == Key.S) { keyboard.DownIsDown = true; }
            if (e.Key == Key.Space) { keyboard.Shooting = true; }
        };
        MainWindow.KeyUp += (s, e) =>
        {
            if (e.Key == Key.A) { keyboard.LeftIsDown = false; }
            if (e.Key == Key.D) { keyboard.RightIsDown = false; }
            if (e.Key == Key.W) { keyboard.UpIsDown = false; }
            if (e.Key == Key.S) { keyboard.DownIsDown = false; }
        };
        var element = new SKElement();
        MainWindow.Content = element;
        element.PaintSurface += (sender, eventArgs) =>
        {
            var canvas = eventArgs.Surface.Canvas;
            if (!game.Paint(canvas, eventArgs.Info, keyboard))
            {
                Timer.Stop();
                MainWindow.Background = Brushes.Black;
                MainWindow.Content = new TextBlock()
                {
                    Text = "Game Over",
                    FontSize = 48,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = Brushes.Red,                };
            }
        };
        Timer.Interval = TimeSpan.FromMilliseconds(1000 / 60);
        Timer.Tick += (s, e) => element.InvalidateVisual();
        Timer.Start();

        MainWindow.Show();
    }

    private void Element_PaintSurface(object? sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
    {
        throw new NotImplementedException();
    }
}

public class KeyboardStatus
{
    public bool LeftIsDown { get; set; }
    public bool RightIsDown { get; set; }
    public bool DownIsDown { get; set; }
    public bool UpIsDown { get; set; }

    public bool Shooting { get; set; }
}