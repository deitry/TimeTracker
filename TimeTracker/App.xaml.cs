using System.Diagnostics;
using SharpHook;
using SharpHook.Native;

#if WINDOWS
using Windows.Graphics;
using ABI.Windows.Devices.Input;
using Microsoft.UI;
using Microsoft.UI.Windowing;
#endif

namespace TimeTracker;

public partial class App : Application
{
#if WINDOWS
    public const int ElementsInPanel = 16;
    public static readonly SizeInt32 HorizontalDefault = new SizeInt32(70 * ElementsInPanel, 50);
    public static readonly SizeInt32 VerticalDefault = new SizeInt32(85, 30 * ElementsInPanel + 15);
#endif

    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();

        _syncContext = SynchronizationContext.Current;
        _hook = new TaskPoolGlobalHook();

        _hook.KeyPressed += OnKeyPressed;
        _hook.KeyReleased += OnKeyReleased;

        if (!Debugger.IsAttached)
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
                // TODO: save current activity before close

                _hook.Dispose();

                Application.Current.CloseWindow(window);
                Application.Current.Quit();
            }
        }
    }
}
