using ClassLibrary.Shared.Enums;

namespace ClassLibrary.Shared.Interfaces;

public interface IServiceResult<T>
{
    T Result { get; set; }
    ServiceStatus Status { get; set; }
}
