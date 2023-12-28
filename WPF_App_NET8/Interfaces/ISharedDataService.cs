using ClassLibrary.Shared.Interfaces;
using System.Collections.ObjectModel;

namespace WPF_App_NET8.Interfaces;

public interface ISharedDataService
{
    ObservableCollection<IContact> ContactList { get; set; }
}
