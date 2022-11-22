using Microsoft.Maui.Controls;
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

    private static void ViewModelOnAlert(TimeTracker tracker)
    {
        Task.Run(async () =>
        {
            var db = await Database.Instance;
            await db.InsertAsync(tracker.ToDb());
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
            MinimumWidth = 185,
        };

        Application.Current?.OpenWindow(window);
    }

    private void ToggleButton_OnToggled(object sender, ToggledEventArgs e)
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
}
