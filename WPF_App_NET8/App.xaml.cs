using ClassLibrary.Shared.Interfaces;
using ClassLibrary.Shared.Models.Responses;
using ClassLibrary.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using WPF_App_NET8.Interfaces;
using WPF_App_NET8.Models;
using WPF_App_NET8.Mvvm.ViewModels;
using WPF_App_NET8.Mvvm.Views;

namespace WPF_App_NET8;
public partial class App : Application
{
    private IHost? _host;

    private readonly string _filePath = @"C:\Exercises\CSharp-Exercise\Contact_WPF.json";

    public App()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddSingleton(typeof(IServiceResult<>), typeof(ServiceResult<>));
                services.AddScoped<IContactService, ContactService>();
                services.AddSingleton<IFileService>(provider => new FileService(_filePath));

                services.AddSingleton<string>(_filePath);

                services.AddSingleton<ISharedDataService, SharedDataService>();


                services.AddTransient<GetContactFromListViewModel>();
                services.AddTransient<GetContactFromListView>();

                services.AddTransient<AddContactToListViewModel>();
                services.AddTransient<AddContactToListView>();

                services.AddTransient<EditContactFromListViewModel>();
                services.AddTransient<EditContactFromListView>();


                services.AddSingleton<MainViewModel>();
                services.AddSingleton<MainWindow>();

            })
            .Build();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        _host!.Start();

        var mainViewModel = _host!.Services.GetRequiredService<MainViewModel>();
        var mainWindow = _host!.Services.GetRequiredService<MainWindow>();

        mainWindow.DataContext = mainViewModel;

        mainWindow.Show();
    }


    protected override void OnExit(ExitEventArgs e)
    {
        _host?.Dispose();
        base.OnExit(e);
    }
}
