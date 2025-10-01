using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using static SettingsService;

namespace LogiSort.Scriptum
{
    public class MainViewModel : INotifyPropertyChanged
    {
         
        private AppSettings _settings;// Текущие настро1ки
        private AppSettings _originalSettings;// Оригинальные настройки (для отмены)
        private readonly LanguageService _languageService;// Сервис языка

        private bool _isSaving;//Флаг сохранения(Показывает идет сохранения)
        private object? _currentView;// Текущее  страница которая показывается в данный момент.
        private ThemeItem? _selectedThemeItem;// Хранит выбранные элементы темы
        private AppTheme _selectedTheme;//ХХранит тёкшую тему
        private double _selectedScale;//Масштаб масштаб
        private bool _isApplyingTheme;

        //==============Команды===================
        public ICommand ApplySettingsCommand { get; private set; }
        public ICommand CancelChangesCommand { get; private set; }
        public ICommand ResetToDefaultsCommand { get; private set; }
        public CommandHandler Commands { get; private set; } = null!;

        // 
        public ReadOnlyObservableCollection<ThemeItem> AvailableThemesLocalized { get; }
        public ObservableCollection<double> AvailableScales { get; } =
            new ObservableCollection<double> { 0.80, 0.90, 0.95, 1.0, 1.10, 1.20, 1.25, 1.30, 1.5 };

        // Сервисы язы
        public LanguageService LanguageService => _languageService;

        //=================Свойства==============================
        public bool IsSaving
        {
            get => _isSaving;
            set => SetProperty(ref _isSaving, value);
        }

        public object? CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public ThemeItem? SelectedThemeItem
        {
            get => _selectedThemeItem;
            set
            {
                if (SetProperty(ref _selectedThemeItem, value) && value != null)
                    ApplyTheme(value.Theme);
            }
        }

        public AppTheme SelectedTheme
        {
            get => _selectedTheme;
            set => ApplyTheme(value);
        }

        public bool IsDarkTheme
        {
            get => SelectedTheme == AppTheme.Dark;
            set => SelectedTheme = value ? AppTheme.Dark : AppTheme.Light;
        }
        public string SelectedLanguage
        {
            get => _languageService.PendingLanguageCode;
            set
            {
                if (_languageService.PendingLanguageCode != value)
                {
                    _languageService.PendingLanguageCode = value;
                    UpdateThemeDisplayNames();
                    OnPropertyChanged();
                }
            }
        }

        public double Volume
        {
            get => _settings.Volume;
            set
            {
                var newValue = Math.Clamp(value, 0.0, 1.0);
                if (_settings.Volume != newValue)
                {
                    _settings.Volume = newValue;
                    OnPropertyChanged();
                }
            }
        }

        public double SelectedScale
        {
            get => _selectedScale;
            set
            {
                if (SetProperty(ref _selectedScale, value))
                {
                    _settings.UiScale = value;
                    ApplyScale(value);
                }
            }
        }
        private void UpdateThemeDisplayNames()
        {
            foreach (var theme in AvailableThemesLocalized)
            {
                if (theme.Theme == AppTheme.Light)
                    theme.DisplayName = _languageService.light_theme;
                else if (theme.Theme == AppTheme.Dark)
                    theme.DisplayName = _languageService.dark_theme;
            }
        }

        //=========================== Конструктор================================================
        public MainViewModel(Window window)
        {
            _originalSettings = SettingsService.LoadSettings() ?? new AppSettings();
            _settings = _originalSettings.Clone();

            _languageService = new LanguageService(_settings);

            //Инициализация команды прямо в конструкторе
            ApplySettingsCommand = new RelayCommand(async () => await SaveSettingsAsync());
            ResetToDefaultsCommand = new RelayCommand(ResetToDefaults);
            CancelChangesCommand = new RelayCommand(CancelChanges);

            Commands = new CommandHandler(window, this);

            _selectedScale = _settings.UiScale;
            ApplyScale(_selectedScale);

            _selectedTheme = _settings.IsDarkTheme ? AppTheme.Dark : AppTheme.Light;
            ApplyTheme(_selectedTheme);

            var themes = new ObservableCollection<ThemeItem>
            {
                new ThemeItem { Theme = AppTheme.Light, DisplayName = LanguageService.light_theme },
                new ThemeItem { Theme = AppTheme.Dark, DisplayName = LanguageService.dark_theme }
            };
            AvailableThemesLocalized = new ReadOnlyObservableCollection<ThemeItem>(themes);
            SelectedThemeItem = AvailableThemesLocalized.First(t => t.Theme == SelectedTheme);

            CurrentView = new Pages.HomeView();
        }

        //===================== Применение темы====================================
        private void ApplyTheme(AppTheme theme)
        {
            if (_isApplyingTheme) return;
            _isApplyingTheme = true;

            _selectedTheme = theme;
            _settings.IsDarkTheme = (theme == AppTheme.Dark);
            ThemeService.ChangeTheme(_settings.IsDarkTheme);

            OnPropertyChanged(nameof(SelectedTheme));
            OnPropertyChanged(nameof(IsDarkTheme));

            _isApplyingTheme = false;
        }

        //=====================Применение масштаба===================================
        private void ApplyScale(double scale)
        {
            if (Application.Current.MainWindow != null)
                Application.Current.MainWindow.LayoutTransform = new ScaleTransform(scale, scale);
        }

        //===========================Сохранение настроек===================================
        private async Task SaveSettingsAsync()
        {
            IsSaving = true;
            try
            {
                await Task.Run(() =>
                {
                    _languageService.ApplyLanguagePermanently();
                    SettingsService.SaveSettings(_settings);
                }).ConfigureAwait(false);

                _originalSettings = _settings.Clone();
                ShowNotification("Настройки сохранены!");
            }//Вывести в отельную и сделать смену язык(Нужна заняться костюмной) 
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка при сохранении настроек: {ex}");
                ShowNotification($"Ошибка: {ex.Message}");
            }
            finally
            {
                IsSaving = false;
            }
        }

        private void ResetToDefaults()
        {
            var defaultSettings = SettingsService.GetDefaultSettings();
            RestoreSettings(defaultSettings);
            ShowNotification("Настройки сброшены к значениям по умолчанию");
        }

        private void CancelChanges()
        {
            RestoreSettings(_originalSettings);
        }

        private void RestoreSettings(AppSettings source)
        {
            _settings = source.Clone();
            _languageService.ResetTo(source.Language);
            SelectedScale = source.UiScale;
            ApplyTheme(source.IsDarkTheme ? AppTheme.Dark : AppTheme.Light);
             
            

            OnPropertyChanged(nameof(SelectedLanguage));
            OnPropertyChanged(nameof(Volume));
            OnPropertyChanged(nameof(SelectedScale));
            UpdateThemeDisplayNames();

        }

        private void ShowNotification(string message)
        {
            MessageBox.Show(message, "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        //============Сеттер для свойства=================
        private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null!)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
                return true;
            }
            return false;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    //========== Клонирования настроек================
    public static class AppSettingsExtensions
    {
        public static AppSettings Clone(this AppSettings source) => new AppSettings
        {
            IsDarkTheme = source.IsDarkTheme,
            Language = source.Language,
            Volume = source.Volume,
            UiScale = source.UiScale
        };
    }

}