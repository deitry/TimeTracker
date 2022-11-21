using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace TimeTracker;

public sealed partial class AlertPage : ContentPage
{
   public AlertPage(TimeTracker tracker)
    {
        InitializeComponent();

        if (this.BindingContext is AlertViewModel vm)
        {
            vm.SetTracker(tracker);
        }
    }

    private void Button_OnClicked(object sender, EventArgs e)
    {
        Application.Current?.CloseWindow(this.Window);
    }
}
