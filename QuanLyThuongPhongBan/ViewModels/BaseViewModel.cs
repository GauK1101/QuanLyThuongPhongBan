using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace QuanLyThuongPhongBan.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        private readonly Dictionary<string, object?> _values = new();

        protected T? Get<T>([CallerMemberName] string propertyName = "")
        {
            if (_values.TryGetValue(propertyName, out var value))
                return (T?)value;
            return default;
        }

        protected bool Set<T>(T value, [CallerMemberName] string propertyName = "")
        {
            if (Equals(Get<T>(propertyName), value))
                return false;

            _values[propertyName] = value;
            OnPropertyChanged(propertyName);
            return true;
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    internal class RelayCommand<T> : ICommand
    {
        private readonly Predicate<T> _canExecute;
        private readonly Action<T> _execute;

        public RelayCommand(Predicate<T> canExecute, Action<T> execute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _canExecute = canExecute;
            _execute = execute;
        }

        public bool CanExecute(object? parameter)
        {
            try
            {
                return _canExecute == null ? true : _canExecute((T)parameter!);
            }
            catch
            {
                return true;
            }
        }

        public void Execute(object? parameter)
        {
            if (parameter is null && typeof(T).IsValueType && Nullable.GetUnderlyingType(typeof(T)) == null)
                throw new ArgumentNullException(nameof(parameter), "Parameter cannot be null for non-nullable value types.");

            _execute((T)parameter!);
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
