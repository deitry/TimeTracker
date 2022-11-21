#nullable enable

namespace TimeTracker;

class ToggleButton : Button
{
    public event EventHandler<ToggledEventArgs>? Toggled;

    public static readonly BindableProperty IsToggledProperty =
        BindableProperty.Create(nameof(IsToggled), typeof(bool), typeof(ToggleButton), false,
            propertyChanged: OnIsToggledChanged);

    public static readonly BindableProperty ToggledColorProperty =
        BindableProperty.Create(nameof(ToggledColor), typeof(Color), typeof(ToggleButton), Color.FromRgb(0, 0, 0));

    public ToggleButton()
    {
        Clicked += (sender, args) => IsToggled ^= true;
    }

    public bool IsToggled
    {
        set { SetValue(IsToggledProperty, value); }
        get { return (bool)GetValue(IsToggledProperty); }
    }

    public Color ToggledColor
    {
        set { SetValue(ToggledColorProperty, value); }
        get { return (Color)GetValue(ToggledColorProperty); }
    }

    protected override void OnParentSet()
    {
        base.OnParentSet();
        VisualStateManager.GoToState(this, "ToggledOff");
    }

    static void OnIsToggledChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ToggleButton toggleButton = (ToggleButton)bindable;
        bool isToggled = (bool)newValue;

        // Fire event
        toggleButton.Toggled?.Invoke(toggleButton, new ToggledEventArgs(isToggled));

        // Set the visual state
        VisualStateManager.GoToState(toggleButton, isToggled ? "ToggledOn" : "ToggledOff");
    }
}
