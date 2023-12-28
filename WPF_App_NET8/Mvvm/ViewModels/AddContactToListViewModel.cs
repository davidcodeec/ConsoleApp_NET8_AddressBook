using ClassLibrary.Shared.Interfaces;
using ClassLibrary.Shared.Models;
using ClassLibrary.Shared.Models.Responses;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
using WPF_App_NET8.Interfaces;

namespace WPF_App_NET8.Mvvm.ViewModels;

public partial class AddContactToListViewModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IContactService _contactService;
    private readonly IFileService _fileService;
    private readonly string _filePath;
    private readonly ISharedDataService _sharedDataService;

    public AddContactToListViewModel(IServiceProvider serviceProvider, IContactService contactService, IFileService fileService, string filePath, ISharedDataService sharedDataService)
    {
        _serviceProvider = serviceProvider;
        _contactService = contactService;
        _fileService = fileService;
        _filePath = filePath;
        _sharedDataService = sharedDataService;
    }

    [ObservableProperty]
    private IContact _contactForm = new Contact();

    private ObservableCollection<IContact> _contactList;


    [RelayCommand]
    private void AddContactToList()
    {
        IServiceResult<IContact> response = new ServiceResult<IContact>();

        try
        {
            if (ContactForm != null! && !string.IsNullOrWhiteSpace(ContactForm.FirstName) &&
               !string.IsNullOrWhiteSpace(ContactForm.LastName) &&
               !string.IsNullOrWhiteSpace(ContactForm.PhoneNumber) &&
               !string.IsNullOrWhiteSpace(ContactForm.Address))
            {

                IContact newContact = new Contact
                {
                    FirstName = ContactForm.FirstName,
                    LastName = ContactForm.LastName,
                    PhoneNumber = ContactForm.PhoneNumber,
                    Email = ContactForm.Email,
                    Address = ContactForm.Address
                };


                // Initialize contactList if it's null or empty
                _contactList ??= _sharedDataService?.ContactList ?? new ObservableCollection<IContact>();

                // Use the IContactService to add the contact
                response = _contactService?.AddContactToList(newContact) ?? response;


                if (response.Status == ClassLibrary.Shared.Enums.ServiceStatus.SUCCESSED)
                {
                    // Clear the textboxes for a new contact
                    ContactForm.FirstName = string.Empty;
                    ContactForm.LastName = string.Empty;
                    ContactForm.Email = string.Empty;
                    ContactForm.PhoneNumber = string.Empty;
                    ContactForm.Address = string.Empty;

                    // Load existing contacts from the file
                    var existingListJson = _fileService.GetContentFromFile(_filePath);
                    var existingList = JsonConvert.DeserializeObject<ObservableCollection<IContact>>(existingListJson) ?? new ObservableCollection<IContact>();

                    // Add the new contact
                    existingList.Add(newContact);

                    // Save the updated list to the file
                    var json = JsonConvert.SerializeObject(existingList, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Objects,
                        Formatting = Formatting.Indented
                    });

                    _fileService.SaveToFile(_filePath, json);

                    // Update the local _contactList to reflect the changes
                    _contactList.Clear();
                    foreach (var contact in existingList)
                    {
                        _contactList.Add(contact);
                    }

                    OnPropertyChanged(nameof(_contactList));
                }
                else
                {
                    response.Status = ClassLibrary.Shared.Enums.ServiceStatus.ALREADY_EXIST;
                }
            }

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            response.Status = ClassLibrary.Shared.Enums.ServiceStatus.FAILED;
            response.Result = ContactForm;
        }
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainViewModel.CurrentViewModel = _serviceProvider.GetRequiredService<GetContactFromListViewModel>();
    }

}
