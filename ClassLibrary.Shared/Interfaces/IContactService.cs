
using ClassLibrary.Shared.Models.Responses;

namespace ClassLibrary.Shared.Interfaces;

public interface IContactService
{
    IServiceResult<IContact> AddContactToList(IContact contact);
    IServiceResult<IEnumerable<IContact>> GetContactFromList();
    IServiceResult<IContact> EditContactFromList(IContact contact);
    IServiceResult<IContact> DeleteContactFromList(IContact contact);
}
