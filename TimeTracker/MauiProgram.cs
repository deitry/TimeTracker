using System.Diagnostics;
using Microsoft.Maui.Platform;
using SharpHook;
using SharpHook.Native;

#if WINDOWS
using Microsoft.Maui.LifecycleEvents;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;
#endif

namespace TimeTracker;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>().ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        });

#if WINDOWS
        builder.ConfigureLifecycleEvents(events =>
        {
            events.AddWindows(windows =>
            {
                windows.OnWindowCreated(window =>
                {
                    IntPtr nativeWindowHandle = WinRT.Interop.WindowNative.GetWindowHandle(window);
                    WindowId nativeWindowId  = Win32Interop.GetWindowIdFromWindow(nativeWindowHandle);
                    AppWindow appWindow = AppWindow.GetFromWindowId(nativeWindowId);

                    window.ExtendsContentIntoTitleBar = false;
                    // var displayInfo = DeviceDisplay.MainDisplayInfo;

                    var size = App.HorizontalDefault;
                    // NOTE: displayInfo.Density is 0 here
                    // size.Width = (int) (size.Width * displayInfo.Density);
                    // size.Height = (int) (size.Height * displayInfo.Density);

                    appWindow.Resize(size);
                    // appWindow.Resize(App.VerticalDefault);

                    var p = appWindow.Presenter as OverlappedPresenter;
                    Debug.Assert(p != null, nameof(p) + " != null");

                    p.SetBorderAndTitleBar(false, false);
                    p.IsMaximizable = false;
                    p.IsMinimizable = false;
                    // p.IsAlwaysOnTop = true;
                });
            });
        });
#endif

        return builder.Build();
    }
}
