using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

using KinoLunticksApp.Models;
using Microsoft.EntityFrameworkCore;
using KinoLunticksApp.Tools;

namespace KinoLunticksApp.Pages
{
    /// <summary>
    /// Interaction logic for AutorizationPage.xaml
    /// </summary>
    public partial class AutorizationPage : Page
    {
        KinoLunticsContext _db = new KinoLunticsContext();
        UserWork _user = new UserWork();

        Frame _frame;

        public AutorizationPage(Frame frame)
        {
            InitializeComponent();

            Data.Login = false;
            _frame = frame;
        }

        private void btnLogIn_Click(object sender, RoutedEventArgs e)
        {
            string login = txtBoxLogin.Text;
            string password = passBoxPassword.Password;

            if (_user.IsValid(login, password))
            {
                Data.Login = true;

                _frame.Navigate(new MainPage(_frame));
            }
            else
            {
                MessageBox.Show(
                    "Неверный логин или пароль. " +
                    "Повторите попытку.",
                    "Ошибка авторизации",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                    );
            }
        }

        private void txtBlockRegistration_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _frame.Navigate(new RegistrationPage(_frame));
        }
    }
}