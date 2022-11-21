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
        _hook = new TaskPoolGlobalHook();

        _hook.KeyPressed += OnKeyPressed;
        _hook.KeyReleased += OnKeyReleased;

        Task.Run(() => _hook.Run());
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        var window = base.CreateWindow(activationState);

        window.Activated += (_, _) => Activated = true;
        window.Deactivated += (_, _) => Activated = false;

        return window;
    }

    public bool Activated { get; set; }

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
    private readonly TaskPoolGlobalHook _hook;

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

        if (WPressed && CtrlPressed && Activated && Application.Current != null)
        {
            var window = Windows.FirstOrDefault();
            if (window != null)
            {
                _hook.Dispose();

                Application.Current.CloseWindow(window);
                Application.Current.Quit();
            }
        }
    }
}
