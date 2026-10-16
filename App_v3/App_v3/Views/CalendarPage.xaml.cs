using App_v3.ViewModels;

namespace App_v3.Views;

public partial class CalendarPage : ContentPage
{
    public CalendarPage(CalendarViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
