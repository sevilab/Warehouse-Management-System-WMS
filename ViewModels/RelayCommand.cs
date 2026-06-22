using System.Windows.Input;

namespace WarehouseManagementSystem.ViewModels
{
    // Command Pattern — UI aksiyonlarını ViewModel'e bağlar
    // ViewModels/RelayCommand.cs
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;// Ne yapılacak
        private readonly Func<object?, bool>? _canExecute;// Yapabilir mi?

        public RelayCommand(Action<object?> execute,
                            Func<object?, bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
        // Butonun aktif/pasif olacağını burası kontrol eder
        public bool CanExecute(object? parameter)
            => _canExecute == null || _canExecute(parameter);

        public void Execute(object? parameter)
            => _execute(parameter);
    }
}