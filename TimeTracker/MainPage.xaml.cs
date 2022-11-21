﻿using Microsoft.Maui.Controls;
#if WINDOWS
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
        _viewModel.Alert += ViewModelOnAlert;
    }

    private async void ViewModelOnAlert(TimeTracker tracker)
    {
        // await DisplayAlert(tracker.Name, $"Elapsed: {tracker.ElapsedTime}", "Ok");
        var window = new Window()
        {
            Page = new AlertPage(tracker),
        };

        window.MinimumHeight = 300;
        window.MinimumWidth = 500;

        Application.Current?.OpenWindow(window);
    }

    private async void ToggleButton_OnToggled(object sender, ToggledEventArgs e)
    {
        if (sender is ToggleButton tb)
        {
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
        }
    }
}
