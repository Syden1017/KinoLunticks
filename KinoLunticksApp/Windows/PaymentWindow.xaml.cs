using System.Windows;
using System.Globalization;
using System.Windows.Input;
using System.Windows.Controls;

using KinoLunticksApp.Pages;
using KinoLunticksApp.Tools;
using KinoLunticksApp.Models;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.ApplicationServices;

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

            if (lViewCards.Items.Count == 1)
            {
                lViewCards.SelectedItem = 0;
            }
        }

        private void btnAddCard_Click(object sender, RoutedEventArgs e)
        {
            AddCardWindow addCard = new AddCardWindow(_orderDetails.authorizedUser);
            addCard.ShowDialog();

            LoadUserCards();
        }

        private void btnPay_Click(object sender, RoutedEventArgs e)
        {
            var selectedCard = lViewCards.SelectedItem as BankAccount;

            SavePaymentToDatabase(selectedCard);
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

        private void SavePaymentToDatabase(BankAccount card)
        {
            var selectedSeats = ParseSelectedSeats(txtBlockSeats.Text);

            using (var _db = new KinoLunticsContext())
            {
                var order = new Order
                {
                    UserId = _orderDetails.authorizedUser.UserId,
                    Showing = _orderDetails.selectedShowing.ShowingId,
                    Amount = Convert.ToDecimal(_orderDetails.fullAmount)
                };

                _db.Orders.Add(order);
                _db.SaveChanges();

                foreach (var seat in selectedSeats)
                {
                    var selectedSeat = new SelectedSeat
                    {
                        OrderId = order.OrderNumber,
                        SeatId = seat.Item1,
                        RowNumber = seat.Item2
                    };

                    _db.SelectedSeats.Add(selectedSeat);
                }

                foreach (var seat in selectedSeats)
                {
                    var takenSeat = new TakenSeat
                    {
                        ShowingId = _orderDetails.selectedShowing.ShowingId,
                        SeatId = seat.Item1,
                        RowNumber = seat.Item2
                    };
                    _db.TakenSeats.Add(takenSeat);
                }

                _db.SaveChanges();
            }

            MessageBox.Show(
                "Оплата прошла успешно.",
                "Оплата заказа",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            Close();
            _frame.Navigate(new MainPage(_frame, _orderDetails.authorizedUser));
        }

        private List<Tuple<int, int>> ParseSelectedSeats(string selectedSeatsString)
        {
            var seats = new List<Tuple<int, int>>();

            if (string.IsNullOrWhiteSpace(selectedSeatsString))
            {
                return seats;
            }

            var parts = selectedSeatsString.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            
            var rowPart = parts[0].Trim();
            if (rowPart.StartsWith("ряд: ") && int.TryParse(rowPart.Substring(5).Trim(), out int rowId))
            {
                var seatsPart = parts[1].Trim();
                if (seatsPart.StartsWith("место: ") || seatsPart.StartsWith("места: "))
                {
                    var seatIds = seatsPart.Substring(seatsPart.StartsWith("место: ") ? 7 : 8).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                        .Select(s => s.Trim());

                    foreach (var seatIdStr in seatIds)
                    {
                        if (int.TryParse(seatIdStr, out int seatId))
                        {
                            seats.Add(new Tuple<int, int>(seatId, rowId));
                        }
                    }
                }
            }

            return seats;
        }
    }
}