
using System.Windows;
using System.Windows.Controls;
using WPF_App_NET8.Mvvm.ViewModels;



namespace WPF_App_NET8.Mvvm.Views;

public partial class EditContactFromListView : UserControl
{

    public EditContactFromListView()
    {
        InitializeComponent();
    }

    private void EditContactFromListView_Loaded(object sender, RoutedEventArgs e)
    {
        // Set focus on the first input control using its x:Name
        Input_FirstName?.Focus();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        ((EditContactFromListViewModel)DataContext)?.ButtonClickHandler();
    }
}
