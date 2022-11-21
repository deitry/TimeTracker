using SharpHook;
using SharpHook.Native;

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

        _syncContext = SynchronizationContext.Current;
        var hook = new TaskPoolGlobalHook();

        hook.KeyPressed += OnKeyPressed;
        hook.KeyReleased += OnKeyReleased;

        Task.Run(() => hook.Run());
    }

    private async void OnKeyReleased(object sender, KeyboardHookEventArgs e)
    {
        await _syncContext;

        switch (e.Data.KeyCode)
        {
            case KeyCode.VcW:
                WPressed = false;
                break;
            case KeyCode.VcLeftControl or KeyCode.VcRightControl:
                CtrlPressed = false;
                break;
        }
    }

    private bool CtrlPressed;
    private bool WPressed;
    private readonly SynchronizationContext _syncContext;

    private async void OnKeyPressed(object sender, KeyboardHookEventArgs e)
    {
        await _syncContext;

        switch (e.Data.KeyCode)
        {
            case KeyCode.VcW:
                WPressed = true;
                break;
            case KeyCode.VcLeftControl or KeyCode.VcRightControl:
                CtrlPressed = true;
                break;
        }

        if (WPressed && CtrlPressed && Application.Current != null)
        {
            foreach (var window in Windows)
            {
                Application.Current.CloseWindow(window);
            }

            Application.Current.Quit();
        }

    }
}
