﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Layouts;
using Application = Microsoft.Maui.Controls.Application;
using Binding = Microsoft.Maui.Controls.Binding;
using Window = Microsoft.Maui.Controls.Window;
#if WINDOWS
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Colors = Microsoft.UI.Colors;
using Windows.Graphics;
using Microsoft.UI;
using Microsoft.UI.Windowing;
#endif

namespace TimeTracker;

public partial class MainPage : ContentPage
{
    private readonly ViewModel _viewModel;

    public MainPage()
    {
        InitializeComponent();

        _viewModel = (BindingContext as ViewModel) !;

        var context = SynchronizationContext.Current!;

        Task.Run(async () =>
        {
            await context;
            await _viewModel.InitializeRunningTrackers();

            await context;
            InitializeLayout();

            _viewModel.Alert += ViewModelOnAlert;
            _viewModel.StartTimer();
        });

        this.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(Title) && this.Window != null)
                this.Window.Title = this.Title;
        };
    }

    private void InitializeLayout()
    {
        var labelTime = new Label();

        labelTime.BindingContext = _viewModel;
        labelTime.SetBinding(Label.TextProperty, nameof(_viewModel.TimeElapsedString));

        MainLayout.Children.Add(labelTime);

        foreach (var category in _viewModel.Categories)
        {
            var button = new ToggleButton
            {
                Text = category.Name,
                ToggledColor = category.ColorObject,
            };

            MainLayout.Children.Add(button);

            button.IsToggled = _viewModel.GetTrackerState(category);
            button.Toggled += ToggleButton_OnToggled;
        }

        var custom = new ToggleButton()
        {
            Text = "Custom",
            // ToggledColor = Microsoft.Maui.Graphics.Colors.LightGray,
            IsEnabled = false,
            BackgroundColor = Color.FromArgb("#242424"),
        };
        // button.Toggled += ToggleButton_OnToggled;
        MainLayout.Children.Add(custom);

        var stats = new Button
        {
            Text = "Stats",
        };
        stats.Clicked += Settings_OnClicked;
        MainLayout.Children.Add(stats);

        MainLayout.Children.Add(new Label()
        {
            Text = "=",
        });

#if WINDOWS
        IntPtr nativeWindowHandle = WinRT.Interop.WindowNative.GetWindowHandle(Window.Handler.PlatformView);
        WindowId nativeWindowId  = Win32Interop.GetWindowIdFromWindow(nativeWindowHandle);
        AppWindow appWindow = AppWindow.GetFromWindowId(nativeWindowId);

        // appWindow.Resize(App.HorizontalDefault);
        // appWindow.Resize(App.VerticalDefault);

        var p = appWindow.Presenter as OverlappedPresenter;
        Debug.Assert(p != null, nameof(p) + " != null");

        p.SetBorderAndTitleBar(false, false);
        p.IsAlwaysOnTop = true;
#endif
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

#if WINDOWS
        // var nativeWindow = Window.Handler.PlatformView;
        // IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
        // WindowId WindowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
        // AppWindow appWindow = AppWindow.GetFromWindowId(WindowId);
        //
        // appWindow.Resize(App.VerticalDefault);
        // // appWindow.TitleBar.BackgroundColor = Colors.Black;
        // // appWindow.TitleBar.ForegroundColor = Colors.Aqua;
        // // appWindow.TitleBar.InactiveBackgroundColor = Colors.Bisque;
        // // appWindow.TitleBar.ButtonBackgroundColor = Colors.Brown;
#endif
    }

    private static void ViewModelOnAlert(TimeTracker tracker)
    {
        Task.Run(async () =>
        {
            var db = await TrackerDatabase.Instance;
            await db.Update(tracker);
        });

        // ShowAlert(tracker);
    }

    private static void ShowAlert(TimeTracker tracker)
    {
        // await DisplayAlert(tracker.Name, $"Elapsed: {tracker.ElapsedTime}", "Ok");

        var window = new Window
        {
            Page = new AlertPage(tracker),
            MinimumHeight = 150,
            // MinimumWidth = 185,
        };

        Application.Current?.OpenWindow(window);
    }

    private void ToggleButton_OnToggled(object? sender, ToggledEventArgs e)
    {
        var context = SynchronizationContext.Current;
        if (sender is ToggleButton tb)
        {
            Task.Run(async () =>
            {
                await context;

                var taskName = tb.Text;
                if (taskName == "Custom" && tb.IsToggled)
                {
                    taskName = await DisplayPromptAsync("New task", "Enter task name:", "Ok", "Cancel", "Custom");
                    if (string.IsNullOrEmpty(taskName))
                    {
                        tb.IsToggled = false;
                        return;
                    }
                }

                _viewModel.Activate(tb.IsToggled, taskName);
            });
        }
    }

    private bool _dragging;
    private Point? _relativePosition;

    private void PointerGestureRecognizer_OnPointerMovedRecognizer_OnPointerMoved(object? sender, PointerEventArgs e)
    {
#if WINDOWS
        if (_dragging)
        {
            var displayInfo = DeviceDisplay.MainDisplayInfo;

            var position = e.GetPosition(null);
            if (position.HasValue)
            {
                _relativePosition ??= position;

                // make absolute position
                var diff = position.Value - _relativePosition.Value;

                var newPosition = new Point(diff.Width + Window.X * displayInfo.Density, diff.Height + Window.Y * displayInfo.Density);

                var nativeWindow = Window.Handler.PlatformView;
                IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
                WindowId WindowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
                AppWindow appWindow = AppWindow.GetFromWindowId(WindowId);

                appWindow.Move(new PointInt32((int)newPosition.X, (int)newPosition.Y));
            }
        }
#endif
    }

    private void TapGestureRecognizer_OnTappedGestureRecognizer_OnTapped(object? sender, TappedEventArgs e)
    {
        _dragging = !_dragging;
        if (!_dragging)
        {
            _relativePosition = null;
        }
    }

    private void TapGestureRecognizer_OnTapped(object? sender, TappedEventArgs e)
    {
        SwitchSize();
    }

    private void SwitchSize()
    {
#if WINDOWS
        var nativeWindow = Window.Handler.PlatformView;
        IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
        WindowId WindowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
        AppWindow appWindow = AppWindow.GetFromWindowId(WindowId);

        var displayInfo = DeviceDisplay.MainDisplayInfo;
        if (Window.Height > Window.Width)
        {
            appWindow.MoveAndResize(new RectInt32(
                _X: 950,
                _Y: 100,
                _Width: (int)(App.HorizontalDefault.Width * displayInfo.Density),
                _Height: (int)(App.HorizontalDefault.Height * displayInfo.Density)));
        }
        else
        {
            appWindow.MoveAndResize(new RectInt32(
                _X: (int)(displayInfo.Width - 80 * displayInfo.Density),
                _Y: (int)(385 * displayInfo.Density),
                _Width: (int)(App.VerticalDefault.Width * displayInfo.Density),
                _Height: (int)(App.VerticalDefault.Height * displayInfo.Density)));
        }
#endif
    }

    private void ClickGestureRecognizer_OnClicked(object? sender, EventArgs e)
    {
        SwitchSize();
    }

    private void Settings_OnClicked(object? sender, EventArgs e)
    {
        if (Application.Current?.Windows.LastOrDefault(w => w.Page is StatisticsPage) != null)
        {
            return;
        }

        var window = new Window
        {
            Page = new StatisticsPage(),
            Height = 300,
            Width = 300,
        };

        Application.Current?.OpenWindow(window);

        var statisticsWindow = Application.Current?.Windows.LastOrDefault(w => w.Page is StatisticsPage);

#if WINDOWS
        if (statisticsWindow != null)
        {
            var nativeWindow = statisticsWindow.Handler.PlatformView;
            IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
            WindowId WindowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
            AppWindow appWindow = AppWindow.GetFromWindowId(WindowId);

            appWindow.Resize(new SizeInt32(500, 400));
            var p = appWindow.Presenter as OverlappedPresenter;
            Debug.Assert(p != null, nameof(p) + " != null");

            p.SetBorderAndTitleBar(true, true);
        }
#endif
    }
}
