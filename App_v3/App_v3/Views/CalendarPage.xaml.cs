using App_v3.ViewModels;

namespace App_v3.Views;

public partial class CalendarPage : ContentPage
{
    public CalendarPage(CalendarViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private void OnBotonMenuFlyoutClicked(object sender, EventArgs e)
    {
        if (Shell.Current != null)
        {
            Shell.Current.FlyoutIsPresented = !Shell.Current.FlyoutIsPresented;
        }
    }
}