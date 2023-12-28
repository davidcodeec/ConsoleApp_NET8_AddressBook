
using System.Windows;
using System.Windows.Controls;

namespace WPF_App_NET8.Mvvm.Views
{

    public partial class AddContactToListView : UserControl
    {
        public AddContactToListView()
        {
            InitializeComponent();
        }

        private void AddContactToListView_Loaded(object sender, RoutedEventArgs e)
        {
            // Set focus on the first input control using its x:Name
            Input_FirstName.Focus();
        }
    }
}
