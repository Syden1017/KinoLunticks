using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

using KinoLunticksApp.Pages;
using KinoLunticksApp.Models;

using Microsoft.EntityFrameworkCore;

namespace KinoLunticksApp.Windows
{
    /// <summary>
    /// Interaction logic for PaymentWindow.xaml
    /// </summary>
    public partial class PaymentWindow : Window
    {
        KinoLunticsContext _db = new KinoLunticsContext();
        User _user = new User();
        Movie _movie = new Movie();

        Frame _frame;

        string _seats;
        string _fullAmount;

        public PaymentWindow(Movie selectedMovie, User user, string seats, string fullAmount, Frame frame)
        {
            InitializeComponent();

            _movie = selectedMovie;
            _user = user;
            _seats = seats;
            _fullAmount = fullAmount;
            _frame = frame;

            DataContext = new
            {
                movieName = _movie.MovieName,
                movieAgeRestriction = _movie.AgeRestriction,
                movieDuration = _movie.MovieDuration,
                seats = _seats,
                fullAmount = _fullAmount
            };

            LoadUserCards();
        }

        private void LoadUserCards()
        {
            _db.Users.Include(u => u.AccountNumbers).FirstOrDefault(u => u.Login == _user.Login);

            lViewCards.ItemsSource = _db.BankAccounts.Local.ToList();
        }

        private void btnPay_Click(object sender, RoutedEventArgs e)
        {
            var order = new Order
            {
                Amount = Convert.ToDecimal(_fullAmount)
            };

            try
            {
                _db.Orders.Add(order);
                _db.SaveChanges();

                MessageBox.Show(
                    "Оплата прошла успешно!",
                    "Проведение транзакции",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                Close();
                _frame.Navigate(new MainPage(_frame, _user));
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
                                          "Отмена оплаты",
                                          MessageBoxButton.YesNo,
                                          MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Close();
            }
        }

        private void lViewCards_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lViewCards.SelectedItem != null)
            {
                btnPay.IsEnabled = true;
                rectTransparent.Visibility = Visibility.Collapsed;
            }
            else
            {
                btnPay.IsEnabled = false;
                rectTransparent.Visibility = Visibility.Visible;
            }
        }

        private void rectTransparent_MouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.No;
            rectTransparent.ToolTip = "Выберите способ оплаты";
        }

        private void rectTransparent_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = null;
        }
    }
}