using System.Windows;
using System.Net.Mail;
using System.Windows.Controls;
using System.Text.RegularExpressions;

using KinoLunticksApp.Pages;
using KinoLunticksApp.Models;

namespace KinoLunticksApp.Tools
{
    class UserWork
    {
        KinoLunticsContext _db = new KinoLunticsContext();

        /// <summary>
        /// Проверяет существование пользователя в базе данных с указанным логином.
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <returns>Возвращает true, если пользователь с заданным логином существует, иначе false.</returns>
        public bool IsLoginExists(string login)
        {
            return _db.Users.Any(user => user.Login == login);
        }

        /// <summary>
        /// Проверяет существование пользователя в базе данных с указанной электронной почтой.
        /// </summary>
        /// <param name="email">Электронная почта пользователя</param>
        /// <returns>Возвращает true, если пользователь с заданной электронной почтой существует, иначе false.</returns>
        public bool IsEmailEsists(string email)
        {
            return _db.Users.Any(user => user.EmailAddress == email);
        }

        /// <summary>
        /// Проверяет правильность введенной электронной почты
        /// </summary>
        /// <param name="mail">Почта для проверки</param>
        /// <returns>Результат проверки</returns>
        public bool isValidMail(string mail)
        {
            var regex = new Regex(@"^(\w+\@\w+\.\w+)$");

            try
            {
                MailAddress m = new MailAddress(mail);
                if (!regex.IsMatch(mail))
                    return false;
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        /// <summary>
        /// Регистрирует нового пользователя в базе данных.
        /// </summary>
        /// <param name="userLogin">Логин пользователя</param>
        /// <param name="password">Пароль пользователя</param>
        /// <param name="userName">Имя пользователя</param>
        /// <param name="userLastName">Фамилия пользователя</param>
        /// <param name="userEmail">Электронная почта пользователя</param>
        /// <param name="frame">Фрейм для навигации на страницу авторизации</param>
        /// <param name="birthDate">Дата рождения пользователя (необязательный параметр)</param>
        public void RegisterUser(string userLogin, string password,
                                 string userName, string userLastName,
                                 string userEmail, Frame frame,
                                 DateOnly? birthDate = null)
        {
            var defaultRole = _db.Roles.FirstOrDefault(role => role.RoleName == "Пользователь");

            User _user = new User
            {
                Login = userLogin,
                Password = password,
                UserName = userName,
                UserLastName = userLastName,
                EmailAddress = userEmail,
                BirthDate = birthDate,
                UserRole = defaultRole.RoleId
            };

            _db.Users.Add(_user);

            try
            {
                _db.SaveChanges();

                MessageBox.Show(
                    "Информация сохранена",
                    "Сохранение",
                     MessageBoxButton.OK,
                     MessageBoxImage.Information
                    );

                frame.Navigate(new AutorizationPage(frame));
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message.ToString(),
                    "Системная ошибка",
                     MessageBoxButton.OK,
                     MessageBoxImage.Error
                    );
            }
        }
    }
}