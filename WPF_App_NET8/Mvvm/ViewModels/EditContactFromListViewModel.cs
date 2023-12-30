using ClassLibrary.Shared.Interfaces;
using ClassLibrary.Shared.Models;
using ClassLibrary.Shared.Models.Responses;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Diagnostics;
using System;
using WPF_App_NET8.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Newtonsoft.Json.Linq;

namespace WPF_App_NET8.Mvvm.ViewModels;

public partial class EditContactFromListViewModel : ObservableObject
{
    [ObservableProperty]
    private IContact _contactForm;

    private IContact _selectedContact;

    private string _selectedContactDisplay;

    private IServiceResult<IContact> _response;

    private ObservableCollection<IContact> _contactList;

    public ICommand EditContactFromListCommand { get; private set; }


    public ObservableCollection<IContact> ContactList
    {
        get => _contactList;
        set
        {
            if (_contactList != value)
            {
                _contactList = value;
                OnPropertyChanged(nameof(ContactList));
            }
        }
    }

    public IContact SelectedContact
    {
        get => _selectedContact;
        set
        {
            if (_selectedContact != value)
            {
                _selectedContact = value;
                OnPropertyChanged(nameof(SelectedContact));
                OnPropertyChanged(nameof(SelectedContactDisplay));
            }
        }
    }


    public string SelectedContactDisplay
    {
        get => _selectedContactDisplay;
        set
        {
            if (_selectedContactDisplay != value)
            {
                _selectedContactDisplay = value;
                OnPropertyChanged(nameof(SelectedContactDisplay)); // Notify property change
            }
        }
    }




    private readonly IServiceProvider _serviceProvider;
    private readonly IContactService _contactService;
    private readonly IFileService _fileService;
    private readonly string _filePath;
    private readonly ISharedDataService _sharedDataService;

    public EditContactFromListViewModel(IServiceProvider serviceProvider, IContactService contactService, IFileService fileService, string filePath, ISharedDataService sharedDataService)
    {
        _serviceProvider = serviceProvider;
        _contactService = contactService;
        _fileService = fileService;
        _filePath = filePath;
        _sharedDataService = sharedDataService;
        _contactForm = new Contact(); // Creates a non-null instance
        EditContactFromListCommand = new RelayCommand(EditContactFromList);
        // Initialize _selectedContact here
        _selectedContact = new Contact();
    }

    private void EditContactFromList()
    {
        _response = new ServiceResult<IContact>();

        try
        {
            // Get the contact list
            var contactListResult = _contactService?.GetContactFromList();

            if (contactListResult?.Status == ClassLibrary.Shared.Enums.ServiceStatus.SUCCESSED)
            {
                var contactList = contactListResult.Result.ToList();

                foreach (var contact in contactList)
                {
                    Debug.WriteLine($"{contact.FirstName} {contact.LastName} - {contact.Email}");
                }

                // Find the index of the selected contact in the list
                var index = -1;

                foreach (var contact in contactList)
                {
                    if (contact.Id == SelectedContact.Id)
                    {
                        index = contactList.IndexOf(contact);
                        break;
                    }
                }

                if (index != -1)
                {
                    // Update the contactList with the modified SelectedContact
                    contactList[index] = SelectedContact;

                    foreach (var contact in contactList)
                    {
                        Debug.WriteLine($"{contact.FirstName} {contact.LastName} - {contact.Email}");
                    }

                    // Serialize the updated list back to JSON
                    var updatedJson = JsonConvert.SerializeObject(contactList, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Objects,
                        Formatting = Formatting.Indented,
                        Converters = { new IContactConverter() }
                    });

                    // Save the updated list to the file
                    _fileService?.SaveToFile(_filePath, updatedJson);
                    Debug.WriteLine("File saved successfully.");

                    _response.Status = ClassLibrary.Shared.Enums.ServiceStatus.SUCCESSED;

                    Debug.WriteLine("Updated selected contact.");
                }
                else
                {
                    // Handle the case where the selected contact is not found in the list
                    _response.Status = ClassLibrary.Shared.Enums.ServiceStatus.NOT_FOUND;
                    Debug.WriteLine("SelectedContact not found in the contact list.");
                }
            }
            else
            {
                // Handle the case where getting the contact list failed
                _response.Status = contactListResult?.Status ?? _response.Status;
                Debug.WriteLine("Failed to retrieve the contact list.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            _response.Status = ClassLibrary.Shared.Enums.ServiceStatus.FAILED;
            _response.Result = SelectedContact;
            Debug.WriteLine($"Error in EditContactFromList: {ex.Message}");
        }

        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = _serviceProvider?.GetRequiredService<GetContactFromListViewModel>();
    }




    public void ButtonClickHandler()
    {
        EditContactFromListCommand.Execute(null);
    }


}
