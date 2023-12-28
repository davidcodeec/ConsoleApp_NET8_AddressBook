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

namespace WPF_App_NET8.Mvvm.ViewModels;

public partial class EditContactFromListViewModel : ObservableObject
{
    [ObservableProperty]
    private IContact _contactForm;

    private IContact _selectedContact;

    private string _selectedContactDisplay;

    private IServiceResult<IContact> _response;

    private ObservableCollection<Contact> _contacts;


    public ObservableCollection<Contact> Contacts
    {
        get => _contacts;
        set
        {
            if (_contacts != value)
            {
                _contacts = value;
                OnPropertyChanged(nameof(Contacts));
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
                OnPropertyChanged(nameof(SelectedContact)); // Notify property change
                SelectedContactDisplay = $"{value?.FirstName} {value?.LastName} - {value?.Email}";
                Debug.WriteLine($"SelectedContact set to: {value?.FirstName} {value?.LastName}");
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
                Debug.WriteLine($"SelectedContactDisplay set to: {value}");
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
        _contactForm = new Contact(); // Make sure this creates a non-null instance

    }



    [RelayCommand]
    private void EditContactFromList()
    {
        _response = new ServiceResult<IContact>();

        try
        {
            Debug.WriteLine("EditContactFromList method executed.");
            Debug.WriteLine($"Is SelectedContact null? {SelectedContact == null}");
            Debug.WriteLine($"SelectedContact: {SelectedContact}");

            if (SelectedContact != null)
            {

                // Get the contact list
                var contactListResult = _contactService?.GetContactFromList();

                if (contactListResult?.Status == ClassLibrary.Shared.Enums.ServiceStatus.SUCCESSED)
                {
                    var contactList = contactListResult.Result.ToList();

                    // Debug statement to print contactList before modifications
                    Debug.WriteLine("ContactList before modifications:");
                    foreach (var contact in contactList)
                    {
                        Debug.WriteLine($"{contact.FirstName} {contact.LastName} - {contact.Email}");
                    }

                    // Find the index of the selected contact in the list
                    var index = contactList.IndexOf(contactList.FirstOrDefault(contact => contact.Email == SelectedContact.Email));

                    if (index != -1)
                    {
                        // Update the contactList with the modified SelectedContact
                        contactList[index] = SelectedContact;

                        // Serialize the updated list back to JSON
                        var updatedJson = JsonConvert.SerializeObject(contactList, Formatting.Indented);

                        // Save the updated list to the file
                        _fileService?.SaveToFile(_filePath, updatedJson);

                        var settings = new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.Objects,
                            Formatting = Formatting.Indented,
                            Converters = { new IContactConverter() }
                        };

                        // Deserialize the updated list back to objects (use the concrete type)
                        var updatedContacts = JsonConvert.DeserializeObject<List<IContact>>(updatedJson, settings);


                        // Update the local _contactList to reflect the changes
                        _sharedDataService.ContactList.Clear();

                        foreach (var contact in updatedContacts)
                        {
                            _sharedDataService.ContactList.Add(contact);
                        }

                        OnPropertyChanged(nameof(_sharedDataService.ContactList));


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
                    Debug.WriteLine("SelectedContact is null.");
                }
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
        Debug.WriteLine($"After executing command: _response = {_response?.Status}");
        Debug.WriteLine("Button Clicked - Triggering EditContactFromListCommand");
    }


}
