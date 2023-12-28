
using ClassLibrary.Shared.Models.Responses;

namespace ClassLibrary.Shared.Interfaces;

public interface IContactService
{

    /// <summary>
    /// Add a contact to a list
    /// </summary>
    /// <param name="contact">a contact of type IContact</param>
    /// <returns></returns>
    IServiceResult<IContact> AddContactToList(IContact contact);

    /// <summary>
    /// Get contact from list
    /// </summary>
    /// <returns></returns>
    IServiceResult<IEnumerable<IContact>> GetContactFromList();

    /// <summary>
    /// Edit a contact from list
    /// </summary>
    /// <param name="contact"></param>
    /// <returns></returns>
    IServiceResult<IContact> EditContactFromList(string existingEmail, IContact editedContact);

    /// <summary>
    /// Delete a contact from list
    /// </summary>
    /// <param name="contact"></param>
    /// <returns></returns>
    IServiceResult<IContact> DeleteContactFromList(IContact contact);
}
