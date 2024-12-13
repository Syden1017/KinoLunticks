using System.Text;
using System.Windows;

using KinoLunticksApp.Models;
using Microsoft.EntityFrameworkCore;

namespace KinoLunticksApp.Windows
{
    /// <summary>
    /// Interaction logic for AddCardWindow.xaml
    /// </summary>
    public partial class AddCardWindow : Window
    {
        KinoLunticsContext _db = new KinoLunticsContext();
        User _user = new User();

        public AddCardWindow(User user)
        {
            InitializeComponent();
            _user = user;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            BankAccount _newAccount = new BankAccount 
            { 
                AccountNumber = txtxBoxAccountNumber.Text,
                ExpirationDate = txtxBoxExpirationDate.Text.Replace(" ", "/"),
                Cvccode = txtxBoxCvccode.Text
            };

            StringBuilder errors = new StringBuilder();

            #region Код карты
            if (_db.BankAccounts.Local.Select(account => account.AccountNumber).ToList().
                        Contains(_newAccount.AccountNumber))
            {
                errors.AppendLine("Карта с этим номером уже привязана к аккаунту!");
            }
            #endregion

            if (errors.Length > 0)
            {
                MessageBox.Show(
                    errors.ToString(),
                    "Заполните поля",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                    );

                return;
            }

            try
            {
                _db.BankAccounts.Add(_newAccount);
                _db.SaveChanges();

                var user = _db.Users.Include(u => u.AccountNumbers).FirstOrDefault(u => u.Login == _user.Login);

                if (user != null)
                {
                    user.AccountNumbers.Add(_newAccount);
                    _db.SaveChanges();
                }

                MessageBox.Show(
                    "Информация сохранена",
                    "Сохранение",
                     MessageBoxButton.OK,
                     MessageBoxImage.Information
                    );

                Close();
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

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                                                    "Вы действительно хотите отменить операцию?",
                                                    "Отмена добавления",
                                                    MessageBoxButton.YesNo,
                                                    MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Close();
            }
        }
    }
}
