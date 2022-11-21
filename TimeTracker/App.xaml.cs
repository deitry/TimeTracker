#if WINDOWS
using Windows.Graphics;
using Microsoft.UI;
using Microsoft.UI.Windowing;
#endif

namespace TimeTracker;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
    }
}
