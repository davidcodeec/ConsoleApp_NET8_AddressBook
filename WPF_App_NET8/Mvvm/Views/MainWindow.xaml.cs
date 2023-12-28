using System.Windows;
using WPF_App_NET8.Mvvm.ViewModels;

namespace WPF_App_NET8.Mvvm.Views;


public partial class MainWindow : Window
{

    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}