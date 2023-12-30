using ClassLibrary.Shared.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using WPF_App_NET8.Interfaces;

namespace WPF_App_NET8.Models;

public class SharedDataService : ISharedDataService, INotifyPropertyChanged
{
    private ObservableCollection<IContact> _contactList = new ObservableCollection<IContact>();

    public ObservableCollection<IContact> ContactList
    {
        get => _contactList;
        set => _contactList = value;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}