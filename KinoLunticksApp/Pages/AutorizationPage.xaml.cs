﻿using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

using KinoLunticksApp.Models;

using Microsoft.EntityFrameworkCore;
using System.IO;

namespace KinoLunticksApp.Pages
{
    /// <summary>
    /// Interaction logic for AutorizationPage.xaml
    /// </summary>
    public partial class AutorizationPage : Page
    {
        KinoLunticsContext _db = new KinoLunticsContext();

        Frame _frame;

        public AutorizationPage(Frame frame)
        {
            InitializeComponent();

            _frame = frame;
        }

        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnLogIn_Click(null, null);
            }
        }

        private void cBoxShowHidePassword_Checked(object sender, RoutedEventArgs e)
        {
            passBoxPassword.Visibility = Visibility.Hidden;
            txtBoxPassword.Visibility = Visibility.Visible;

            txtBoxPassword.Text = passBoxPassword.Password;
            cBoxShowHidePassword.Content = "Скрыть пароль";
        }

        private void cBoxShowHidePassword_Unchecked(object sender, RoutedEventArgs e)
        {
            txtBoxPassword.Visibility = Visibility.Hidden;
            passBoxPassword.Visibility = Visibility.Visible;

            passBoxPassword.Password = txtBoxPassword.Text;
            cBoxShowHidePassword.Content = "Показать пароль";
        }

        private void btnLogIn_Click(object sender, RoutedEventArgs e)
        {
            var user = _db.Users.AsNoTracking().FirstOrDefault(user => user.Login == txtBoxLogin.Text);

            if (user == null)
            {
                MessageBox.Show(
                    "Неверный логин. " +
                    "Повторите попытку.",
                    "Ошибка авторизации",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                    );
            }

            if (BCrypt.Net.BCrypt.Verify(passBoxPassword.Password, user.Password))
            {
                if (user.UserRole.ToString() == "2")
                {
                    _frame.Navigate(new MainPage(_frame, user));
                }
                else
                {
                    _frame.Navigate(new AdminPanelPage(_frame));
                }
            }
            else
            {
                MessageBox.Show(
                    "Неверный пароль. " +
                    "Повторите попытку.",
                    "Ошибка авторизации",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                    );
            }
        }

        private void txtBlockRegistration_MouseDown(object sender, MouseButtonEventArgs e) => _frame.Navigate(new RegistrationPage(_frame));
    }
}