using System.ComponentModel;
using System.Runtime.CompilerServices;
// MVVM Pattern — tüm ViewModel'lerin türeyeceği temel sınıf
// OOP: Abstraction + Inheritance
// INotifyPropertyChanged — UI'ı otomatik günceller

namespace WarehouseManagementSystem.ViewModels
{
   //TEMEL SINIF 
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value,
            [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
