using Avalonia;
using AvaloniaFireworks;

AppBuilder.Configure<App>()
    .UsePlatformDetect()
    .LogToTrace()
    .StartWithClassicDesktopLifetime(args);
