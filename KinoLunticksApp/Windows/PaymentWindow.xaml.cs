using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

using KinoLunticksApp.Pages;
using KinoLunticksApp.Tools;
using KinoLunticksApp.Models;

using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace KinoLunticksApp.Windows
{
    /// <summary>
    /// Interaction logic for PaymentWindow.xaml
    /// </summary>
    public partial class PaymentWindow : Window
    {
        KinoLunticsContext _db = new KinoLunticsContext();

        OrderDetails _orderDetails;
        Frame _frame;

        public PaymentWindow(OrderDetails details)
        {
            InitializeComponent();

            _frame = details.navigationFrame;
            _orderDetails = details;

            DataContext = new
            {
                _orderDetails.selectedMovie.Preview,
                _orderDetails.selectedMovie.MovieName,
                _orderDetails.selectedMovie.AgeRestriction,
                formattedDate = _orderDetails.selectedShowing.ShowingDate.ToString("d MMMM", CultureInfo.CurrentCulture),
                _orderDetails.selectedShowing.ShowingTime,
                hall = LoadHallData().HallNumber,
                seat = _orderDetails.selectedPlaces,
                _orderDetails.fullAmount
            };

            LoadUserCards();
        }

        private Hall LoadHallData()
        {
            var hallId = _db.Showings.Where(s => s.ShowingDate == _orderDetails.selectedShowing.ShowingDate &&
                                                 s.ShowingTime == _orderDetails.selectedShowing.ShowingTime).
                                      Select(s => s.HallId).FirstOrDefault();

            var hall = _db.Halls.FirstOrDefault(h => h.HallId == hallId);

            return hall;
        }

        private void LoadUserCards()
        {
            _db.Users.Include(u => u.AccountNumbers).
                      FirstOrDefault(u => u.Login == _orderDetails.authorizedUser.Login);

            lViewCards.ItemsSource = _db.BankAccounts.Local.ToList();
        }

        private void btnPay_Click(object sender, RoutedEventArgs e)
        {
            //var order = new Order
            //{
            //    Amount = Convert.ToDecimal(_fullAmount)
            //};

            //try
            //{
            //    _db.Orders.Add(order);
            //    _db.SaveChanges();

            //    MessageBox.Show(
            //        "Оплата прошла успешно!",
            //        "Проведение транзакции",
            //        MessageBoxButton.OK,
            //        MessageBoxImage.Information);

            //    Close();

            //    _frame.Navigate(new MainPage(_frame, _user));
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(
            //        ex.Message.ToString(),
            //        "Системная ошибка",
            //        MessageBoxButton.OK,
            //        MessageBoxImage.Error
            //        );
            //}
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

        private void btnAddCard_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}