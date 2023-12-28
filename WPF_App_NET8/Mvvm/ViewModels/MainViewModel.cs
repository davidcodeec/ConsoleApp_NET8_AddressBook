
using CommunityToolkit.Mvvm.ComponentModel;

using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using WPF_App_NET8.Interfaces;


namespace WPF_App_NET8.Mvvm.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ISharedDataService _sharedDataService;
        private ObservableObject _currentViewModel;

        public MainViewModel(IServiceProvider serviceProvider, ISharedDataService sharedDataService)
        {
            _serviceProvider = serviceProvider;
            CurrentViewModel = _serviceProvider.GetRequiredService<GetContactFromListViewModel>();
            _sharedDataService = sharedDataService;
        }

        public ObservableObject CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                if (_currentViewModel != value)
                {
                    _currentViewModel = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
