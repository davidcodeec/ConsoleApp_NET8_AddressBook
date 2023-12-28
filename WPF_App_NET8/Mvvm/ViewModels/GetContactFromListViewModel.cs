using ClassLibrary.Shared.Interfaces;
using ClassLibrary.Shared.Models.Responses;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace WPF_App_NET8.Mvvm.ViewModels;

public partial class GetContactFromListViewModel : ObservableObject
{

    private readonly IServiceProvider _serviceProvider;
    private readonly IContactService _contactService;
    private readonly IFileService _fileService;
    private readonly string _filePath;

    public GetContactFromListViewModel(IServiceProvider serviceProvider, IContactService contactService, IFileService fileService, string filePath)
    {
        _serviceProvider = serviceProvider;
        _contactService = contactService;
        _fileService = fileService;
        _filePath = filePath;
        GetContactFromList();
    }

    [ObservableProperty]
    private ObservableCollection<IContact> _contactList = new ObservableCollection<IContact>();

    private IServiceResult<IEnumerable<IContact>> GetContactFromList()
    {
        IServiceResult<IEnumerable<IContact>> response = new ServiceResult<IEnumerable<IContact>>();

        try
        {
            _contactList.Clear();
            var content = _fileService.GetContentFromFile(_filePath);
            if (!string.IsNullOrEmpty(content))
            {
                _contactList = JsonConvert.DeserializeObject<ObservableCollection<IContact>>(content, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                    Formatting = Formatting.Indented
                })!;
            }
            response.Status = ClassLibrary.Shared.Enums.ServiceStatus.SUCCESSED;
            response.Result = _contactList;
            OnPropertyChanged(nameof(_contactList));
            //Console.WriteLine("Contacts retrieved successfully in GetCustomersFromList");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            response.Status = ClassLibrary.Shared.Enums.ServiceStatus.FAILED;
            response.Result = Enumerable.Empty<IContact>();
        }

        return response;
    }



    [RelayCommand]
    private void NavigateToAddContactToList()
    {
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = _serviceProvider.GetRequiredService<AddContactToListViewModel>();
    }

    [RelayCommand]
    private void NavigateToEditContactFromList(IContact selectedContact)
    {
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = _serviceProvider.GetRequiredService<EditContactFromListViewModel>();
    }

    [RelayCommand]
    private void RemoveContactFromList(IContact contact)
    {
        IServiceResult<IContact> response = new ServiceResult<IContact>();

        try
        {

            if (contact != null)
            {
                response = _contactService.DeleteContactFromList(contact);

                if (response.Status == ClassLibrary.Shared.Enums.ServiceStatus.SUCCESSED)
                {
                    _contactList.Remove(contact);
                    var json = JsonConvert.SerializeObject(_contactList, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Objects,
                        Formatting = Formatting.Indented
                    });

                    _fileService.SaveToFile(_filePath, json);
                    OnPropertyChanged(nameof(_contactList));
                }
                else
                {
                    // Handle the failure scenario if needed
                    Debug.WriteLine($"Failed to remove contact. Status: {response.Status}");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            response.Status = ClassLibrary.Shared.Enums.ServiceStatus.FAILED;
        }
    }
}
