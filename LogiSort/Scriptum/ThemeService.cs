using MaterialDesignThemes.Wpf;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace LogiSort.Scriptum
{
    public enum AppTheme
    {
        Light,
        Dark
    }
    public class ThemeItem : INotifyPropertyChanged
    {
        public AppTheme Theme { get; set; }

        private string _Namedisplay = string.Empty;
        public string DisplayName
        {
            get => _Namedisplay;
            set
            {
                if (_Namedisplay != value)
                {
                    _Namedisplay = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayName)));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

}
public static class ThemeService
{
    public static void LoadTheme()
    {
        var settings = SettingsService.LoadSettings();
        ApplyTheme(settings.IsDarkTheme);
    }

    public static void ChangeTheme(bool isDark)
    {
        ApplyTheme(isDark);
    }

    private static void ApplyTheme(bool isDark)
    {
        var paletteTheme = new PaletteHelper();
        var theme = paletteTheme.GetTheme();
        theme.SetBaseTheme(isDark ? BaseTheme.Dark : BaseTheme.Light);
        paletteTheme.SetTheme(theme);

        //Меняет цвет фона  
        Application.Current.Resources["CustomBackground"] = isDark
            ? Application.Current.Resources["CustomBackgroundDark"]
            : Application.Current.Resources["CustomBackgroundLight"];
    }
}
