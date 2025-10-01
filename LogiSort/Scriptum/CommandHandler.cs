using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Navigation;
 

namespace LogiSort.Scriptum
{
    public class CommandHandler
    {
        public ICommand CloseCommand { get; }//Закрыть
        public ICommand ToggleSizeCommand { get; } //Свернуть развернуть програму
        public ICommand MoveWindowCommand { get; }//Не исподзованая заготовка
        public ICommand CommandHidesProgram { get; }//Cкрыть окно

        // Команды для навигации
        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateSettingsCommand { get; }

        public ICommand GalleryCommand { get; }

        private readonly MainViewModel _viewModel;//Ссылка на MainViewModel, чтобы менять CurrentView.



        public CommandHandler(Window window, MainViewModel viewModel)
        {
            _viewModel = viewModel;
            CloseCommand = new RelayCommand(() => window.Close());//Закрыть
            //Свернуть развернуть програму
            ToggleSizeCommand = new RelayCommand(() =>
            {
                if (window.WindowState == WindowState.Normal)
                {
                    // Устанавливаем окно в максимизированное состояние
                    window.WindowState = WindowState.Maximized;
                }
                else
                {
                    // Возвращаем окно в обычное состояние
                    window.WindowState = WindowState.Normal;
                }
            });
            CommandHidesProgram = new RelayCommand(() => window.WindowState = WindowState.Minimized);//Cкрыть окно
            MoveWindowCommand = new RelayCommand(() => { /* Логика перемещения окна */ });
            // Навигационные команды
            NavigateHomeCommand = new RelayCommand(() => _viewModel.CurrentView = new Pages.HomeView());//Устанавливает текущую страницу в HomeView.
            NavigateSettingsCommand = new RelayCommand(() => _viewModel.CurrentView = new Pages.SettingsView());//Устанавливает текущую страницу в SettingsView.
            GalleryCommand = new RelayCommand(() => _viewModel.CurrentView = new Pages.GalleryPage());//Устанавливает текущую страницу в SettingsView.
        }
        
    }
}
