using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Input;
using LogiSort.Properties;
using static SettingsService;

namespace LogiSort.Scriptum
{

    //Перевод хранится в ресурсах
    public class LanguageService : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        // Хранит текущие настройки приложения
        private readonly AppSettings _settings;

        // Текущая культура (язык)
        private CultureInfo _currentCulture;

        // Кэш загруженных строк для разных языков
        private readonly Dictionary<string, Dictionary<string, string>> _languageCache = new();

        // Текущие строки для активного языка
        private Dictionary<string, string> _currentStrings = new();

        // Список доступных языков
        public List<LanguageOption> AvailableLanguages { get; } = new()
        {
            new LanguageOption("Русский (Russian)", "ru-RU"),
            new LanguageOption("English", "en-US")
        };

        // Временный код языка (до сохранения)
        private string _pendingLanguageCode;

        /// <summary>
        /// Временный код языка для привязки в UI
        /// </summary>
        public string PendingLanguageCode
        {
            get => _pendingLanguageCode;
            set
            {
                if (_pendingLanguageCode != value)
                {
                    _pendingLanguageCode = value;
                    ApplyTemporarily(); // Применяем временно
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Конструктор инициализирует сервис языка
        /// </summary>
        public LanguageService(AppSettings settings)
        {
            _settings = settings;

            // Устанавливаем текущий язык из настроек
            _currentCulture = new CultureInfo(_settings.Language);
            Thread.CurrentThread.CurrentUICulture = _currentCulture;

            // Инициализируем временный язык
            _pendingLanguageCode = _settings.Language;

            // Загружаем ресурсы
            UpdateResources();
        }

        /// <summary>
        /// Временное применение языка (без сохранения)
        /// </summary>
        private void ApplyTemporarily()
        {
            _currentCulture = new CultureInfo(_pendingLanguageCode);
            Thread.CurrentThread.CurrentUICulture = _currentCulture;
            UpdateResources();
        }

        /// <summary>
        /// Фиксация языка (при сохранении настроек)
        /// </summary>
        public void ApplyLanguagePermanently()
        {
            // Сохраняем в настройки
            _settings.Language = _pendingLanguageCode;

            // Обновляем культуру
            _currentCulture = new CultureInfo(_pendingLanguageCode);
            Thread.CurrentThread.CurrentUICulture = _currentCulture;

            // Обновляем ресурсы
            UpdateResources();
        }

        /// <summary>
        /// Сброс к указанному языку (для отмены изменений)
        /// </summary>
        public void ResetTo(string languageCode)
        {
            // Обновляем временный код
            _pendingLanguageCode = languageCode;

            // Устанавливаем культуру
            _currentCulture = new CultureInfo(languageCode);
            Thread.CurrentThread.CurrentUICulture = _currentCulture;

            // Обновляем ресурсы
            UpdateResources();
        }

        // Свойства для привязки строк в UI
        //Главная страница 
        public string MainPageString0 => GetString("Main_page_String_0");
        public string MainButtonSettings1 => GetString("MainButtonSettings_1");
        //Настройки
        public string SettingsLanguage => GetString("SettingsLanguage");
        public string SettingsLanguagechange => GetString("SettingsLanguagechange");
        public string Page_Title_Settings => GetString("Page_Title_Settings");
        public string SettAutomatic_updates => GetString("SettAutomatic_updates");
        public string SettCancel => GetString("SettCancel");
        public string SettChange_of_topic => GetString("SettChange_of_topic");
        public string SettCheck_for_updates => GetString("SettCheck_for_updates");
        public string SettEnable_logging => GetString("SettEnable_logging");
        public string SettFont => GetString("SettFont");
        public string SettLogging => GetString("SettLogging");
        public string SettNotifications => GetString("SettNotifications");
        public string SettOpen_log => GetString("SettOpen_log");
        public string SettReset_settings => GetString("SettReset_settings");
        public string SettSave => GetString("SettSave");
        public string SettSound => GetString("SettSound");
        public string SettTopic => GetString("SettTopic");
        public string SettUpdates => GetString("SettUpdates");
        public string SettVisual => GetString("SettVisual");
        //Домашняя страница
        public string HomeScan_folder => GetString("HomeScan_folder");
        public string HomeSort => GetString("HomeSort");
        public string HomeWelcome_text => GetString("HomeWelcome_text");
        ///////////////////////////////////////////////////////////////////////////////////////
        public string dark_theme => GetString("dark_theme");
        public string light_theme => GetString("light_theme");

        ///////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Получает строку по ключу
        /// </summary>
        private string GetString(string key) =>
            _currentStrings.TryGetValue(key, out var value) ? value : string.Empty;

        /// <summary>
        /// Загружает строки для указанной культуры
        /// </summary>
        private void LoadStringsForCulture(string culture)
        {
            if (!_languageCache.ContainsKey(culture))
            {
                _languageCache[culture] = new Dictionary<string, string>
                {
                    //Главная страница
                    { "Main_page_String_0", Resources.Main_page_String_0 },
                    { "MainButtonSettings_1", Resources.MainButtonSettings_1 },
                    //Настройки
                    { "SettingsLanguage", Resources.SettingsLanguage },
                    { "SettingsLanguagechange", Resources.SettingsLanguagechange},
                    { "Page_Title_Settings", Resources.Page_Title_Settings},
                    { "SettAutomatic_updates", Resources.SettAutomatic_updates},
                    { "SettCancel", Resources.SettCancel},
                    { "SettChange_of_topic", Resources.SettChange_of_topic},
                    { "SettCheck_for_updates", Resources.SettCheck_for_updates},
                    { "SettEnable_logging", Resources.SettEnable_logging},
                    { "SettFont", Resources.SettFont},
                    { "SettLogging", Resources.SettLogging},
                    { "SettNotifications", Resources.SettNotifications},
                    { "SettOpen_log", Resources.SettOpen_log},
                    { "SettReset_settings", Resources.SettReset_settings},
                    { "SettSave", Resources.SettSave},
                    { "SettSound", Resources.SettSound},
                    { "SettTopic", Resources.SettTopic},
                    { "SettUpdates", Resources.SettUpdates},
                    { "SettVisual", Resources.SettVisual},
                    //Домашняя страница
                    { "HomeScan_folder", Resources.HomeScan_folder },
                    { "HomeSort", Resources.HomeSort },
                    { "HomeWelcome_text", Resources.HomeWelcome_text },
                    //
                    { "dark_theme", Resources.dark_theme },
                    { "light_theme", Resources.light_theme }
                };
            }
            _currentStrings = _languageCache[culture];
        }

        /// <summary>
        /// Обновляет все ресурсы и уведомляет UI
        /// </summary>
        private void UpdateResources()
        {
            LoadStringsForCulture(_currentCulture.Name);

            // Уведомляем об изменении всех строк
            //Главная страница
            OnPropertyChanged(nameof(MainPageString0));
            OnPropertyChanged(nameof(MainButtonSettings1));
            //Настройки
            OnPropertyChanged(nameof(SettingsLanguage));
            OnPropertyChanged(nameof(SettingsLanguagechange));
            OnPropertyChanged(nameof(Page_Title_Settings));
            OnPropertyChanged(nameof(SettAutomatic_updates));
            OnPropertyChanged(nameof(SettCancel));
            OnPropertyChanged(nameof(SettChange_of_topic));
            OnPropertyChanged(nameof(SettCheck_for_updates));
            OnPropertyChanged(nameof(SettEnable_logging));
            OnPropertyChanged(nameof(SettFont));
            OnPropertyChanged(nameof(SettLogging));
            OnPropertyChanged(nameof(SettNotifications));
            OnPropertyChanged(nameof(SettOpen_log));
            OnPropertyChanged(nameof(SettReset_settings));
            OnPropertyChanged(nameof(SettSave));
            OnPropertyChanged(nameof(SettSound));
            OnPropertyChanged(nameof(SettTopic));
            OnPropertyChanged(nameof(SettUpdates));
            OnPropertyChanged(nameof(SettVisual));
            //Домашняя страница
            OnPropertyChanged(nameof(HomeScan_folder));
            OnPropertyChanged(nameof(HomeSort));
            OnPropertyChanged(nameof(HomeWelcome_text));
            //
            OnPropertyChanged(nameof(dark_theme));
            OnPropertyChanged(nameof(light_theme));
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Класс для хранения информации о языке
    /// </summary>
    public record LanguageOption(string Name, string Code);

}