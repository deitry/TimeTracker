namespace TimeTracker;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private void OnCounterClicked(object sender, EventArgs e)
    {
        if (BindingContext is ViewModel vm)
        {
            vm.Activate();
        }

        // SemanticScreenReader.Announce(CounterBtn.Text);
    }
}
