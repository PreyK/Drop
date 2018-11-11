using Avalonia;
using Avalonia.Markup.Xaml;

namespace Drop
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
