using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

using KinoLunticksApp.Models;

using Microsoft.EntityFrameworkCore;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace KinoLunticksApp.Pages
{
    /// <summary>
    /// Interaction logic for AutorizationPage.xaml
    /// </summary>
    public partial class AutorizationPage : Page
    {
        KinoLunticsContext _db = new KinoLunticsContext();

        Frame _frame;

        private bool isPasswordVisible = false;

        public AutorizationPage(Frame frame)
        {
            InitializeComponent();

            _frame = frame;
        }

        private void btnLogIn_Click(object sender, RoutedEventArgs e)
        {
            string login = txtBoxLogin.Text;
            string password = passBoxPassword.Password;

            var user = _db.Users.AsNoTracking().FirstOrDefault(user => user.Login == login && user.Password == password);

            if (user == null)
            {
                MessageBox.Show(
                    "Неверный логин или пароль. " +
                    "Повторите попытку.",
                    "Ошибка авторизации",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                    );
            }

            if (user.UserRole == "002")
            {
                _frame.Navigate(new MainPage(_frame, user));
            }
            else
            {
                _frame.Navigate(new AdminPanelPage(_frame));
            }
        }

        private void txtBlockRegistration_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _frame.Navigate(new RegistrationPage(_frame));
        }
    }
}