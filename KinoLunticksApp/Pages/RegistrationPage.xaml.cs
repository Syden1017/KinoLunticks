using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;

using KinoLunticksApp.Models;
using KinoLunticksApp.Tools;
using System.Text;

namespace KinoLunticksApp.Pages
{
    /// <summary>
    /// Interaction logic for RegistrationPage.xaml
    /// </summary>
    public partial class RegistrationPage : Page
    {
        KinoLunticsContext _db = new KinoLunticsContext();
        UserWork _user = new UserWork();

        Frame _frame;

        public RegistrationPage(Frame frame)
        {
            InitializeComponent();

            _frame = frame;
        }

        private void btnSignIn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            string login = txtBoxLogin.Text;
            string password = passBoxPassword.Password;
            string confirmPassword = passBoxConfirmPassword.Password;
            string userName = txtBoxFirstName.Text;
            string userLastName = txtBoxLastName.Text;
            string email = txtBoxEmail.Text;

            if (password != confirmPassword)
            {
                errors.AppendLine("Пароли не совпадают. Повторите попытку");
            }

            if (_user.IsLoginExists(login))
            {
                errors.AppendLine("Пользователь с данным логином зарегистрирован в системе.");
            }

            if (_user.IsEmailEsists(email))
            {
                errors.AppendLine("Пользователь с данной электронной почтой зарегистрирован в системе.");
            }

            if (errors.Length > 0)
            {
                MessageBox.Show(
                    errors.ToString(),
                    "Ошибка регистрации",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                    );

                return;
            }
            else
            {
                _user.RegisterUser(login, password, userName, userLastName, email, _frame);
            }

        }

        private void txtBlockAuthorization_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _frame.Navigate(new AutorizationPage(_frame));
        }
    }
}