using ClassLibrary.Shared.Enums;
using ClassLibrary.Shared.Interfaces;
using ClassLibrary.Shared.Models;
using ClassLibrary.Shared.Services;
using Moq;
using Newtonsoft.Json;

namespace ConsoleApp_NET8.Tests;

public class ContactService_Tests
{
    private const string FilePath = @"C:\Exercises\CSharp-Exercise\Test.json";

    [Fact]
    public void AddToListShould_AddOneContactToContactList_ThenReturnTrue() 
    {
        // Arrange
        var filePath = FilePath;
        IContact contact = new Contact();

        var mockFileService = new Mock<IFileService>();

        IContactService contactService = new ContactService(mockFileService.Object,filePath);

        // Act
        IServiceResult<IContact> result = contactService.AddContactToList(contact);

        // Assert
        Assert.True(result.Status == ServiceStatus.SUCCESSED);
    }

    [Fact]
    public void GetAllFromListShould_GetAllContactsInContactList_ThenReturnListOfContacts() 
    {
        // Arrange
        var filePath = FilePath;
        var existingContacts = new List<Contact> { new Contact { Id = new Guid("27bb9cfe-0473-4449-835c-4d0e486eb35d"), FirstName = "John", LastName = "John", PhoneNumber = "01000222" , Email = "john@john.se" , Address = "Johns Address" } };

        var json = JsonConvert.SerializeObject(existingContacts, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects,
            Formatting = Formatting.Indented
        });

        var mockFileService = new Mock<IFileService>();

        IContactService contactsService = new ContactService(mockFileService.Object, filePath);

        // Mock the behavior of _fileService to return a known set of contacts
        mockFileService.Setup(fs => fs.GetContentFromFile(It.IsAny<string>())).Returns(json);


        // Act
        IServiceResult<IEnumerable<IContact>> result = contactsService.GetContactFromList();

        // Assert
        Assert.Equal(ServiceStatus.SUCCESSED, result.Status);

        if (result.Status == ServiceStatus.SUCCESSED)
        {
            IEnumerable<IContact> contacts = result.Result as IEnumerable<IContact>;

            // Additional assertions based on the returned contacts, e.g., checking count
            Assert.NotNull(contacts);
            Assert.True(contacts.Any()); // Check if there is at least one contact

            // Example: Assert that there are at least two contacts in the collection
            Assert.True(contacts.Count() >= 1, "Expected at least one contact.");

            // Example: Assert that all contacts have a non-null email
            Assert.True(contacts.All(c => !string.IsNullOrEmpty(c.Email)), "Some contacts have a null or empty email.");
        }
        else
        {
            // Handle the failure case
            // Optionally assert any specific conditions for failure
            Assert.True(false, $"Test failed. Status: {result.Status}, Error: {result.Result}");
        }
    }


    [Fact]
    public void EditContactFromList_EditOneContactFromContactList_ThenReturnTrue()
    {
        // Arrange
        var filePath = FilePath;
        var contactService = new ContactService(new FileService(filePath), filePath);

        var existingContact = new Contact
        {
            Id = new Guid("27bb9cfe-0473-4449-835c-4d0e486eb35d"),
            FirstName = "John",
            LastName = "John",
            PhoneNumber = "01000222",
            Email = "john@john.se",
            Address = "Johns Address"
        };

        // Add the contact to the list
        var addResult = contactService.AddContactToList(existingContact);
        Assert.True(addResult.Status == ServiceStatus.SUCCESSED);

        // Act: Edit the contact
        var editedContact = new Contact
        {
            Id = existingContact.Id,
            FirstName = "UpdatedJohn",
            LastName = "UpdatedJohn",
            PhoneNumber = "01000222",
            Email = "john@john.se",
            Address = "Updated Johns Address"
        };

        var editResult = contactService.EditContactFromList(existingContact.Email, editedContact);

        // Assert
        Assert.True(editResult.Status == ServiceStatus.SUCCESSED);

        // Act: Retrieve the contacts and find the edited contact
        var getResult = contactService.GetContactFromList();

        // Assert
        Assert.True(getResult.Status == ServiceStatus.SUCCESSED);

        // Check that the contact was actually edited
        var editedContactFromList = getResult.Result.FirstOrDefault(c => c.Email == editedContact.Email);
        Assert.NotNull(editedContactFromList);
        Assert.Equal("UpdatedJohn", editedContactFromList.FirstName);
    }






    [Fact]
    public void DeleteContactFromList_DeleteOneContactFromContactList_ThenReturnTrue()
    {
        // Arrange
        var filePath = FilePath;
        var contactService = new ContactService(new FileService(filePath), filePath);


        var existingContact = new Contact
        {
            Id = new Guid("27bb9cfe-0473-4449-835c-4d0e486eb35d"),
            FirstName = "John",
            LastName = "John",
            PhoneNumber = "01000222",
            Email = "john@john.se",
            Address = "Johns Address"
        };

        // Act
        var addResult = contactService.AddContactToList(existingContact);
        var deleteResult = contactService.DeleteContactFromList(existingContact);
        var getResult = contactService.GetContactFromList();

        // Assert
        Assert.True(addResult.Status == ServiceStatus.SUCCESSED);
        Assert.True(deleteResult.Status == ServiceStatus.SUCCESSED);
        Assert.True(getResult.Status == ServiceStatus.SUCCESSED);

        // Check that the contact no longer exists
        bool contactExists = getResult.Result.Any(x => x.Email == existingContact.Email);
        Assert.False(contactExists);
    }


}
