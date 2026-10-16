using App_v3.ViewModels;
namespace App_v3.Views;

public partial class AttendancePage : ContentPage
{
    public AttendancePage(AttendanceViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}



