using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

using KinoLunticksApp.Tools;
using KinoLunticksApp.Models;

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

        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnSignIn_Click(null, null);
            }
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

            if (login.Length > 0)
            {
                if (_user.IsLoginExists(login))
                {
                    errors.AppendLine("Пользователь с данным логином зарегистрирован в системе. " +
                                      "Если вы уже зарегистрированы, войдите в систему.");
                }

                if (_user.IsEmailEsists(email))
                {
                    errors.AppendLine("Пользователь с данной электронной почтой зарегистрирован в системе. " +
                                      "Если вы уже зарегистрированы, войдите в систему.");
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

                if (password.Length > 0)
                {
                    if (password.Length > 6)
                    {
                        if (password == confirmPassword)
                        {
                            bool en = true;
                            bool number = false;

                            for (int i = 0; i < password.Length; i++)
                            {
                                if (password[i] >= 'А' && password[i] <= 'Я') en = false;
                                if (password[i] >= '0' && password[i] <= '9') number = true;
                            }
                            if (!en) MessageBox.Show("Пароль должен состоять только из английских букв");
                            else if (!number) MessageBox.Show("Пароль должен содержать хотя бы одну цифру");
                            else if (!_user.isValidMail(email))
                                MessageBox.Show("Введите корректный e-mail.", "Ошибка!", MessageBoxButton.OK);

                            if (en && number && _user.isValidMail(email))
                            {
                                _user.RegisterUser(login, BCrypt.Net.BCrypt.HashPassword(password),
                                                   userName, userLastName,
                                                   email, _frame);
                            }
                        }
                        else
                        {
                            MessageBox.Show(
                                "Пароли не совпадают. " +
                                "Повторите попытку",
                                "Ошибка регистрации",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning
                                );
                        }
                    }
                    else
                    {
                        MessageBox.Show(
                            "Пароль должен быть не короче 6 символов",
                            "Ошибка регистрации",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning
                            );
                    }
                }
                else
                {
                    MessageBox.Show(
                            "Введите пароль",
                            "Ошибка регистрации",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning
                            );
                }
            }
            else
            {
                MessageBox.Show(
                        "Введите логин",
                        "Ошибка регистрации",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                        );
            }
        }

        private void txtBlockAuthorization_MouseDown(object sender, MouseButtonEventArgs e) => _frame.Navigate(new AutorizationPage(_frame));
    }
}