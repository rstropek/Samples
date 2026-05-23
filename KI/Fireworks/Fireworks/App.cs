using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;

namespace AvaloniaFireworks;

public class App : Application
{
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is ClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                Width = 800,
                Height = 600,
                Title = "Avalonia Fireworks"
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
