using System;
using System.Globalization;
using System.IO;
using System.Windows;
using Newtonsoft.Json;

public static class SettingsService
{
    public static AppSettings GetDefaultSettings()
    {
        return new AppSettings
        {
            IsDarkTheme = false,
            Language = "ru-RU",  //Русский язык по умолчанию
            Volume = 1.0,
            UiScale = 1.0,
        };

    }
    public class AppSettings
    {
        private double _volume = 1.0;
        private string _language = "ru";

        public bool IsDarkTheme { get; set; }

         
        public string Language
        {
            get => _language;
            set => _language = IsValidLanguage(value) ? value : "ru";
        }

        public double Volume
        {
            get => _volume;
            set => _volume = Math.Clamp(value, 0.0, 1.0);
        }
        public double UiScale 
        { 
            get; 
            set; 
        } = 1.0;//Моштаб 100%
        private static bool IsValidLanguage(string code)
        {
            var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            return cultures.Any(c => c.Name == code);
        }
    }
    private static readonly Mutex _mutex = new Mutex(false, "Global\\SettingsServiceMutex");

    public static void SaveSettings(AppSettings settings)
    {
        _mutex.WaitOne();
        try
        {
            string json = JsonConvert.SerializeObject(settings);
            File.WriteAllText(filePath, json);
        }
        finally
        {
            _mutex.ReleaseMutex();
        }
    }

    private static readonly string filePath = "settings.json";

    //Загрузка настроек из файла
    public static AppSettings LoadSettings()
    {
        if (!File.Exists(filePath))
        {
            return new AppSettings(); //Если файла нет, создает  настройки по умолчанию
        }

        try
        {
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<AppSettings>(json) ?? new AppSettings();
        }
        catch
        {
            return new AppSettings();//Если ошибка,возвращаеет по умолчанию
        }
    }

    //Сохранение настроек в файл 
    private static bool _flagDirty;
    public static void SaveSettings(AppSettings settings, bool force = false)
    {
        if (!force && !_flagDirty) return;

        try
        {
            string json = JsonConvert.SerializeObject(settings);
            File.WriteAllText(filePath, json);
            _flagDirty = false;
        }
        catch (Exception ex)
        {
            // Логирование ошибки(В будущем) 
            MessageBox.Show("eror");
        }
    }
}
