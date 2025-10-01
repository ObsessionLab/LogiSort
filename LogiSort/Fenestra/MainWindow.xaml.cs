 
using LogiSort.Scriptum;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LogiSort;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
     
    public MainViewModel ViewModel { get; }

    public MainWindow()
    {
        InitializeComponent();
        ViewModel = new MainViewModel(this);
        DataContext = ViewModel; // Устанавливает MainViewModel как источник данных ViewModel
        //Ограничение на размер окна 
        this.MinWidth = 1140;
        this.MinHeight = 700;

    }
    //Правила развертки окна при зажатие 
    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2) // двойной клик мыши — разворот/сворачивание
        {
            if (this.WindowState == WindowState.Normal)
                this.WindowState = WindowState.Maximized;
            else
                this.WindowState = WindowState.Normal;
        }
        else if (e.LeftButton == MouseButtonState.Pressed)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal; // сначала вернуть
            }
            this.DragMove();  // Перемещение окна при зажатой левой кнопке мыши
        }
    }
    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
            this.Close();
    }
    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        IntPtr handle = new WindowInteropHelper(this).Handle;
        HwndSource.FromHwnd(handle)?.AddHook(WindowProc);
    }
    private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        const int WM_NCHITTEST = 0x0084;// куда попала мышь
        const int HTLEFT = 10;         // мышь у левого края
        const int HTRIGHT = 11;        // мышь у правого края
        const int HTTOP = 12;          // мышь у верхнего края
        const int HTTOPLEFT = 13;      // мышь в левом верхнем углу
        const int HTTOPRIGHT = 14;     // мышь в правом верхнем углу
        const int HTBOTTOM = 15;       // мышь у нижнего края
        const int HTBOTTOMLEFT = 16;   // мышь в левом нижнем углу
        const int HTBOTTOMRIGHT = 17;  // мышь в правом нижнем углу

        if (msg == WM_NCHITTEST)
        {
            //преобразуем координаты мышь
            Point pos = PointFromScreen(new Point(
                (lParam.ToInt32() & 0xFFFF),
                (lParam.ToInt32() >> 16)));

            int resizeBorder = 20; // толщина зоны угла

            // проверяем, попали ли в край
            if (pos.Y < resizeBorder)
            {
                handled = true;
                if (pos.X < resizeBorder) return (IntPtr)HTTOPLEFT;
                if (pos.X > ActualWidth - resizeBorder) return (IntPtr)HTTOPRIGHT;
                return (IntPtr)HTTOP;
            }
            else if (pos.Y > ActualHeight - resizeBorder)
            {
                handled = true;
                if (pos.X < resizeBorder) return (IntPtr)HTBOTTOMLEFT;
                if (pos.X > ActualWidth - resizeBorder) return (IntPtr)HTBOTTOMRIGHT;
                return (IntPtr)HTBOTTOM;
            }
            else if (pos.X < resizeBorder)
            {
                handled = true;
                return (IntPtr)HTLEFT;
            }
            else if (pos.X > ActualWidth - resizeBorder)
            {
                handled = true;
                return (IntPtr)HTRIGHT;
            }
        }

        // если не попали в зону resize → ничего не трогаем
        return IntPtr.Zero;
    }

}