using ClassLibrary.Shared.Interfaces;
using ClassLibrary.Shared.Models;

namespace ClassLibrary.Shared.Services;


public interface IMenuService
{
    void ShowMainMenu();

}

public class MenuService : IMenuService
{

    private readonly IContactService _contactService;

    public MenuService(IContactService contactService)
    {
        _contactService = contactService;
    }

    public void ShowMainMenu()
    {
        while (true)
        {
            DisplayMenuTitle("MENU OPTION");
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine($"{"Step 1.",-4} Add New Contact Information");
            Console.WriteLine($"{"Step 2.",-4} View Contact List");
            Console.WriteLine($"{"Step 3.",-4} Edit Contact List");
            Console.WriteLine($"{"Step 4.",-4} Remove Contact From List By Email");
            Console.WriteLine($"{"0.",-4} Exit Application");
            Console.WriteLine();
            Console.Write("Enter Menu Option: ");
            var option = Console.ReadLine();
            Console.WriteLine($"Selected option: {option}");

            switch (option)
            {
                case "1":
                    ShowAddContactOption();
                    break;
                case "2":
                    ShowViewContactListOption();
                    break;
                case "3":
                    ShowEditContactOption();
                    break;
                    case "4":
                    ShowDeleteContactOption();
                    break;
                case "0":
                    ShowExitApplicationOption();
                    break;
                default:
                    Console.WriteLine("\nInvalid option Selected. Press any key to try again");
                    Console.ReadKey(); // To end the while loop
                    break;
            }
        }
    }


    private void ShowExitApplicationOption()
    {
        Console.Clear();
        Console.Write("Are you sure you want to close the application? (y/n): ");
        var option = Console.ReadLine() ?? ""; // If not is pressed use empty string

        if (option.Equals("y", StringComparison.CurrentCultureIgnoreCase))
        {
            Environment.Exit(0);
        }
    }



    private void ShowAddContactOption()
    {
        IContact contact = new Contact();
        DisplayMenuTitle("Add New Customer");
        Console.Write("First Name: ");
        contact.FirstName = Console.ReadLine()!;

        Console.Write("Last Name: ");
        contact.LastName = Console.ReadLine()!;

        Console.Write("E-mail: ");
        contact.Email = Console.ReadLine()!;

        Console.Write("Phone Number: ");
        contact.PhoneNumber = Console.ReadLine()!;

        Console.Write("Address: ");
        contact.Address = Console.ReadLine()!;

        var res = _contactService.AddContactToList(contact);

        switch (res.Status)
        {
            case Enums.ServiceStatus.SUCCESSED:
                Console.WriteLine("The customer was added successfully! ");
                break;

            case Enums.ServiceStatus.ALREADY_EXIST:
                Console.WriteLine("The customer already exist! ");
                break;

            case Enums.ServiceStatus.FAILED:
                Console.WriteLine("Failed when trying to add the customer to the list! ");
                Console.WriteLine("See error message : " + res.Result.ToString());
                break;
        }

        Console.ReadKey(intercept: true);

        DisplayPressAnyKey("Press any key to continue.");
    }


    private void ShowDeleteContactOption()
    {
        Console.Clear();
        DisplayMenuTitle("Delete Contact");

        var contactsResult = _contactService.GetContactFromList();
        var contacts = contactsResult.Result as List<IContact>;

        if (contacts.Any())
        {
            Console.WriteLine("Contact List:");
            Console.WriteLine("------------------------");

            foreach (var contact in contacts)
            {
                Console.WriteLine($"{contact.FirstName} {contact.LastName} <{contact.Email}>");
            }

            Console.WriteLine("------------------------");
        }
        else
        {
            Console.WriteLine("No contacts found.");
        }

        Console.Write("Enter the email of the contact to delete: ");
        var emailToDelete = Console.ReadLine();

        if (!string.IsNullOrEmpty(emailToDelete))
        {
            IContact contactToDelete = new Contact { Email = emailToDelete };
            var result = _contactService.DeleteContactFromList(contactToDelete);

            switch (result.Status)
            {
                case Enums.ServiceStatus.SUCCESSED:
                    Console.WriteLine("Contact deleted successfully!");
                    break;

                case Enums.ServiceStatus.NOT_FOUND:
                    Console.WriteLine("Contact not found!");
                    break;

                case Enums.ServiceStatus.FAILED:
                    Console.WriteLine("Failed to delete contact. See error message: " + result.Result.ToString());
                    break;
            }
        }
        else
        {
            Console.WriteLine("Invalid email. Deletion canceled.");
        }

        Console.ReadKey(intercept: true);
        DisplayPressAnyKey("Press any key to continue.");
    }

    private void ShowEditContactOption()
    {
        Console.Clear();

        var contactsResult = _contactService.GetContactFromList();

        if (contactsResult.Status == Enums.ServiceStatus.SUCCESSED)
        {
            var contacts = contactsResult.Result as List<IContact>;

            if (contacts.Any())
            {
                Console.WriteLine("Contact List:");
                Console.WriteLine("------------------------");

                foreach (var contact in contacts)
                {
                    Console.WriteLine($"{contact.FirstName} {contact.LastName} <{contact.Email}>");
                }

                Console.WriteLine("------------------------");

                Console.Write("Enter the email of the contact you want to edit: ");
                var email = Console.ReadLine();

                var contactToEdit = contacts.FirstOrDefault(c => c.Email == email);

                if (contactToEdit != null)
                {
                    Console.WriteLine("Edit Contact:");
                    Console.WriteLine("------------------------");

                    Console.Write($"First Name ({contactToEdit.FirstName}): ");
                    var firstName = Console.ReadLine();
                    contactToEdit.FirstName = string.IsNullOrEmpty(firstName) ? contactToEdit.FirstName : firstName;

                    Console.Write($"Last Name ({contactToEdit.LastName}): ");
                    var lastName = Console.ReadLine();
                    contactToEdit.LastName = string.IsNullOrEmpty(lastName) ? contactToEdit.LastName : lastName;

                    Console.Write($"Email ({contactToEdit.Email}): ");
                    var editedEmail = Console.ReadLine();
                    contactToEdit.Email = string.IsNullOrEmpty(editedEmail) ? contactToEdit.Email : editedEmail;

                    Console.Write($"Phone Number ({contactToEdit.PhoneNumber}): ");
                    var phoneNumber = Console.ReadLine();
                    contactToEdit.PhoneNumber = string.IsNullOrEmpty(phoneNumber) ? contactToEdit.PhoneNumber : phoneNumber;

                    Console.Write($"Address ({contactToEdit.Address}): ");
                    var address = Console.ReadLine();
                    contactToEdit.Address = string.IsNullOrEmpty(address) ? contactToEdit.Address : address;

                    // Save the edited contact
                    var editResult = _contactService.EditContactFromList(contactToEdit.Email, contactToEdit);

                    if (editResult.Status == Enums.ServiceStatus.SUCCESSED)
                    {
                        Console.WriteLine("Contact edited successfully!");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to edit contact. Status: {editResult.Status}");
                    }
                }
                else
                {
                    Console.WriteLine($"Contact with email '{email}' not found.");
                }
            }
            else
            {
                Console.WriteLine("No contacts found.");
            }
        }
        else
        {
            Console.WriteLine($"Failed to retrieve contacts. Status: {contactsResult.Status}");
        }

        Console.ReadKey(intercept: true);
        DisplayPressAnyKey("Press any key to continue.");
    }



    private void ShowViewContactListOption()
    {
        Console.WriteLine("Entering ShowViewCustomerListOption");
        DisplayMenuTitle("Customer List");
        var res = _contactService.GetContactFromList();

        if (res.Status == Enums.ServiceStatus.SUCCESSED)
        {
            //Console.WriteLine("Customers retrieved successfully");
            if (res.Result is List<IContact> contactList)
            {
                if (!contactList.Any())
                {
                    Console.WriteLine("No customers found!");
                }
                else
                {
                    foreach (var contact in contactList)
                    {
                        Console.WriteLine("------------------------");
                        Console.WriteLine($"FirstName : {contact.FirstName}");
                        Console.WriteLine($"LastName : {contact.LastName}");
                        Console.WriteLine($"Email : <{contact.Email}>");
                        Console.WriteLine($"Phone Number : {contact.PhoneNumber}");
                        Console.WriteLine($"Address: {contact.Address}");
                        Console.WriteLine("------------------------");
                        Console.WriteLine();
                    }
                }
            }
        }

        //Console.ReadKey(intercept: true);
        Console.ReadKey();

        DisplayPressAnyKey("Press any key to continue.");

    }

    private void DisplayMenuTitle(string title)
    {
        Console.Clear();
        Console.WriteLine($"################### {title} ###################");
        //Console.WriteLine();
    }

    private void DisplayPressAnyKey(string message)
    {
        Console.WriteLine(message);
        //Console.ReadKey(intercept: true);
        Console.ReadKey();
    }
}
