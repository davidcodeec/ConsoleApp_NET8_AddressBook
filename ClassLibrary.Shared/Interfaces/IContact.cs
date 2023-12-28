using System.ComponentModel;

namespace ClassLibrary.Shared.Interfaces;

public interface IContact : INotifyPropertyChanged
{
    Guid Id { get; set; }
    string FirstName { get; set; }
    string LastName { get; set; }
    string PhoneNumber { get; set; }
    string Email { get; set; }
    string Address { get; set; }
}
