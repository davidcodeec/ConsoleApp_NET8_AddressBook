using ClassLibrary.Shared.Enums;
using ClassLibrary.Shared.Interfaces;
using ClassLibrary.Shared.Models.Responses;
using Newtonsoft.Json;
using System.Diagnostics;

namespace ClassLibrary.Shared.Services;

public class ContactService : IContactService
{
    private List<IContact> _contacts = new List<IContact>();
    private readonly string _filePath;
    private readonly IFileService _fileService;

    public ContactService(IFileService fileService, string filePath)
    {
        _fileService = fileService;
        _filePath = filePath;
    }


    public IServiceResult<IContact> AddContactToList(IContact contact)
    {
        IServiceResult<IContact> response = new ServiceResult<IContact>();

        try
        {
            if (!_contacts.Any(x => x.Email == contact.Email))
            {
                ///contact.Id = _contacts.Count + 1;
                _contacts.Add(contact);
                response.Status = Enums.ServiceStatus.SUCCESSED;

                var json = JsonConvert.SerializeObject(_contacts, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                    Formatting = Formatting.Indented
                });

                _fileService.SaveToFile(_filePath, json);
            }
            else
            {
                response.Status = Enums.ServiceStatus.ALREADY_EXIST;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            response.Status = Enums.ServiceStatus.FAILED;
            response.Result = contact; 
        }

        return response;
    }

    public IServiceResult<IEnumerable<IContact>> GetContactFromList()
    {
        IServiceResult<IEnumerable<IContact>> response = new ServiceResult<IEnumerable<IContact>>();

        try
        {
            var content = _fileService.GetContentFromFile(_filePath);
            if(!string.IsNullOrEmpty(content))
            {
                _contacts = JsonConvert.DeserializeObject<List<IContact>>(content, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                    Formatting = Formatting.Indented
                })!;
            }
            response.Status = Enums.ServiceStatus.SUCCESSED;
            response.Result = _contacts;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            response.Status = Enums.ServiceStatus.FAILED;
            response.Result = Enumerable.Empty<IContact>();
        }

        return response;
    }

    public IServiceResult<IContact> EditContactFromList(string existingEmail, IContact editedContact)
    {
        IServiceResult<IContact> response = new ServiceResult<IContact>();

        try
        {
            if (!string.IsNullOrEmpty(existingEmail) && editedContact != null)
            {
                var existingContact = _contacts.FirstOrDefault(x => x.Email == existingEmail);

                if (existingContact != null)
                {

                    // Remove the existing contact from the list
                    _contacts.Remove(existingContact);

                    // Update the properties of the existing contact with the edited contact
                    existingContact.Email = editedContact.Email;
                    existingContact.FirstName = editedContact.FirstName;
                    existingContact.LastName = editedContact.LastName;
                    existingContact.PhoneNumber = editedContact.PhoneNumber;
                    existingContact.Address = editedContact.Address;

                    // Add the edited contact back to the list
                    _contacts.Add(existingContact);

                    response.Status = Enums.ServiceStatus.SUCCESSED;

                    // Save the updated list to the file
                    var json = JsonConvert.SerializeObject(_contacts, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Objects,
                        Formatting = Formatting.Indented
                    });

                    _fileService.SaveToFile(_filePath, json);
                }
                else
                {
                    response.Status = Enums.ServiceStatus.NOT_FOUND;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            response.Status = Enums.ServiceStatus.FAILED;
            response.Result = editedContact;
        }

        return response;
    }


    public IServiceResult<IContact> DeleteContactFromList(IContact contact)
    {
        IServiceResult<IContact> response = new ServiceResult<IContact>();

        try
        {
            if (!_contacts.Contains(contact) || string.IsNullOrEmpty(contact.Email))
            {
                var existingContact = _contacts.FirstOrDefault(x => x.Email.Trim().Equals(contact.Email.Trim(), StringComparison.OrdinalIgnoreCase));

                if (existingContact != null)
                {
                    _contacts.Remove(existingContact);
                    response.Status = Enums.ServiceStatus.SUCCESSED;
                }
                else
                {
                    response.Status = Enums.ServiceStatus.NOT_FOUND;
                }

                // Save the updated list to the file regardless of whether the contact was found or not.
                var json = JsonConvert.SerializeObject(_contacts, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                    Formatting = Formatting.Indented
                });

                _fileService.SaveToFile(_filePath, json);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            response.Status = Enums.ServiceStatus.FAILED;
        }

        return response;
    }



}
