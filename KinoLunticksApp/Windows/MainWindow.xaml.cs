using System.Windows;
using System.Windows.Input;

using KinoLunticksApp.Pages;

namespace KinoLunticksApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            frmMain.Navigate(new AutorizationPage(frmMain));
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
            }
            else
            {
                WindowState = WindowState.Normal;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result =
                MessageBox.Show(
                    "Вы действительно хотите выйти из программы?",
                    "Завершение работы",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                    );

            if (result == MessageBoxResult.Yes)
            {
                Close();
            }
        }

        private void frmMain_ContentRendered(object sender, EventArgs e)
        {

        }
    }
}