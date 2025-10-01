using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LogiSort.Scriptum
{
    public class RelayCommand : ICommand
    {
        private readonly Action  _execute;
        private readonly Func<bool>? _canExecute;//Определяет,можно ли выполнить команду (по умолчанию true)

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;//Проверка на возможность выполнение
        public void Execute(object? parameter) => _execute();//Выполняем команду

        //Обновление кнопок в  WPF  
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
