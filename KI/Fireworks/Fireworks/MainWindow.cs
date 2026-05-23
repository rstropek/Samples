using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Controls.Skia;
using Avalonia.Threading;
using SkiaSharp;

namespace AvaloniaFireworks;

public class MainWindow : Window
{
    private readonly DispatcherTimer _timer;
    private readonly DispatcherTimer _fireworkTimer;
    private readonly List<Rocket> _fireworks = [];

    public MainWindow()
    {
        _fireworks.Add(new Rocket());

        var canvasControl = new SKCanvasControl();
        canvasControl.Draw += (sender, e) =>
        {
            try
            {
                var bounds = e.Canvas.LocalClipBounds;
                var width = bounds.Width;
                var height = bounds.Height;

                e.Canvas.Clear(SKColors.Black);

                foreach (var rocket in _fireworks)
                {
                    rocket.Draw(e.Canvas);
                }

                _fireworks.RemoveAll(f => f.State == RocketState.Done);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            };
        };
        Content = canvasControl;

        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1.0 / 60) };
        _timer.Tick += (sender, e) => canvasControl.InvalidateVisual();
        _timer.Start();

        // Add a random firework every 5 seconds
        _fireworkTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(0.5) };
        _fireworkTimer.Tick += (sender, e) =>
        {
            _fireworks.Add(new Rocket());
        };
        _fireworkTimer.Start();
    }
}
