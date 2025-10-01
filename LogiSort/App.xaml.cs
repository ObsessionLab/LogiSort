using System.Configuration;
using System.Data;
using System.Windows;
using LogiSort.Scriptum;

namespace LogiSort;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    //Загружаем тему при запуске
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Загружаем тему из JSON
        ThemeService.LoadTheme();
    }

}

