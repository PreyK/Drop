using System;
using Avalonia;
using Avalonia.Logging.Serilog;
using Avalonia.Platform;
using Drop.Networking;

namespace Drop
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            BuildAvaloniaApp().Start<MainWindow>();
        }
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToDebug();

        private static void ConfigureAssetAssembly(AppBuilder builder)
        {
            AvaloniaLocator.CurrentMutable
                .GetService<IAssetLoader>()
                .SetDefaultAssembly(typeof(App).Assembly);
        }

    }

    }

