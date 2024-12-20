﻿using System.Text;
using System.Windows;
using System.Globalization;
using System.Windows.Media;
using System.Windows.Controls;
using System.Text.RegularExpressions;

using KinoLunticksApp.Tools;
using KinoLunticksApp.Models;
using KinoLunticksApp.Windows;

using Microsoft.EntityFrameworkCore;

namespace KinoLunticksApp.Pages
{
    /// <summary>
    /// Interaction logic for MoviePage.xaml
    /// </summary>
    public partial class MoviePage : Page
    {
        KinoLunticsContext _db = new KinoLunticsContext();

        SessionDetails _sessionDetails;
        Frame _frame;

        private List<Seat> _selectedPlaces;
        private decimal _ticketAmount;

        public MoviePage(SessionDetails details)
        {
            InitializeComponent();

            _frame = details.navigationFrame;
            _sessionDetails = details;

            _selectedPlaces = new List<Seat>();
            _ticketAmount = 0;

            UpdateSelectedPlacesTextBox(_selectedPlaces, txtBoxSelected);
            UpdateTicketAmountTextBox();
            PopulateHall(LoadHallData());

            DataContext = new
            {
                _sessionDetails.selectedMovie.Preview,
                _sessionDetails.selectedMovie.MovieName,
                _sessionDetails.selectedMovie.Genres,
                _sessionDetails.selectedMovie.AgeRestriction,
                _sessionDetails.selectedMovie.MovieDuration,
                hall = LoadHallData().HallNumber,
                formattedShowingDate = _sessionDetails.selectedShowing.ShowingDate.ToString("d MMMM", CultureInfo.CurrentCulture),
                _sessionDetails.selectedShowing.ShowingTime,
                _sessionDetails.authorizedUser
            };
        }

        #region Hall creating
        private Hall LoadHallData()
        {
            var hallId = _db.Showings.AsNoTracking().
                                    Where(s => s.ShowingDate == _sessionDetails.selectedShowing.ShowingDate &&
                                             s.ShowingTime == _sessionDetails.selectedShowing.ShowingTime).
                                    Select(s => s.HallId).FirstOrDefault();

            var hall = _db.Halls.Include(h => h.Rows).
                                ThenInclude(r => r.Seats).
                             FirstOrDefault(h => h.HallId == hallId);

            return hall;
        }

        private void PopulateHall(Hall hall)
        {
            gHallSchema.Children.Clear();
            gHallSchema.RowDefinitions.Clear();
            gHallSchema.ColumnDefinitions.Clear();

            var rowsList = hall.Rows.ToList();

            for (int i = 0; i < rowsList.Count; i++)
            {
                gHallSchema.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            }

            int maxSeatsCount = 0;
            for (int i = 0; i < rowsList.Count; i++)
            {
                if (i == rowsList.Count - 1)
                {
                    maxSeatsCount = 10;
                }
                else
                {
                    maxSeatsCount = Math.Max(maxSeatsCount, rowsList[i].Seats.Count);
                }
            }

            for (int j = 0; j < maxSeatsCount + 1; j++)
            {
                gHallSchema.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }

            for (int i = 0; i < rowsList.Count; i++)
            {
                var row = rowsList[i];

                TextBlock rowNumberTextBlock = new TextBlock
                {
                    Text = row.RowNumber.ToString(),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(5),
                    Foreground = new SolidColorBrush(Colors.White),
                    FontSize = 18
                };
                Grid.SetRow(rowNumberTextBlock, i);
                Grid.SetColumn(rowNumberTextBlock, 0);
                gHallSchema.Children.Add(rowNumberTextBlock);

                int seatCount = (i == rowsList.Count - 1) ? 10 : row.Seats.Count;

                for (int j = 0; j < seatCount; j++)
                {
                    Seat seat;
                    if (i == rowsList.Count - 1)
                    {
                        seat = new Seat { SeatNumber = (j + 1).ToString() };
                    }
                    else
                    {
                        seat = row.Seats.ElementAt(j);
                    }

                    Button seatButton = new Button
                    {
                        Name = $"Seat_{row.RowId}_{seat.SeatId}",
                        Content = seat.SeatNumber,
                        Style = (Style)FindResource("GreenArmchairButtonStyle")
                    };

                    seatButton.Click += Button_Click;

                    Grid.SetRow(seatButton, i);
                    Grid.SetColumn(seatButton, (i == rowsList.Count - 1) ? j + 1 : j + 2);

                    gHallSchema.Children.Add(seatButton);
                }
            }

            CheckAndColorReservedSeats(_sessionDetails.selectedShowing.ShowingId, _sessionDetails.selectedShowing.HallId);
        }

        private List<Order> GetOrdersByShowId(int showId, int hallId)
        {
            return _db.Orders
                .Where(o => o.ShowingNavigation.ShowingId == showId && o.ShowingNavigation.HallId == hallId)
                .ToList();
        }

        private void CheckAndColorReservedSeats(int showId, int hallId)
        {
            var orders = GetOrdersByShowId(showId, hallId);

            if (orders != null && orders.Any())
            {
                foreach (var order in orders)
                {
                    var selectedSeats = order.SelectedSeats;
                    selectedSeats = Regex.Replace(selectedSeats, @"\s+", " ");
                    selectedSeats = selectedSeats.Replace("\r\n", "").Replace("\n", "").Replace("\r", "");

                    var rowMatch = Regex.Match(selectedSeats, @"Ряд\s*(\d+)");
                    var seatsMatch = Regex.Match(selectedSeats, @"(?:место|места):\s*([\d\s,]+)");

                    if (rowMatch.Success && seatsMatch.Success)
                    {
                        var seatNumbers = seatsMatch.Groups[1].Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                                    .Select(s => s.Trim())
                                                                    .Select(int.Parse)
                                                                    .ToList();

                        var rowId = _db.Rows.FirstOrDefault(r => r.HallId == hallId && r.RowNumber == rowMatch.ToString())?.RowId;

                        if (rowId.HasValue)
                        {
                            foreach (var seatNumber in seatNumbers)
                            {
                                var seat = _db.Seats.FirstOrDefault(s => s.RowId == rowId.Value && s.SeatNumber == seatNumber.ToString());
                                if (seat != null)
                                {
                                    var button = GetButtonForSeat(rowId.Value, seat.SeatId);
                                    if (button != null)
                                    {
                                        button.Style = (Style)FindResource("GrayArmchairButtonStyle");
                                        button.IsEnabled = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private Button GetButtonForSeat(int rowId, int seatId)
        {
            return gHallSchema.Children.OfType<Button>()
                                        .FirstOrDefault(b => b.Name == $"Seat_{rowId}_{seatId}");
        }
        #endregion

        #region User experience methods
        private void UpdateSelectedPlacesTextBox(List<Seat> selectedSeats, TextBox selectedSeatsTextBox)
        {
            selectedSeatsTextBox.Clear();

            if (selectedSeats.Count == 0)
            {
                selectedSeatsTextBox.Text = "Нет выбранных мест.";
                return;
            }

            var groupedSeats = selectedSeats.GroupBy(seat => seat.Row);
            StringBuilder result = new StringBuilder();

            foreach (var group in groupedSeats)
            {
                Row row = group.Key as Row;

                if (row != null)
                {
                    string rowNumber = row.RowNumber;
                    var seatNumbers = group.Select(seat => seat.SeatNumber).ToList();

                    if (seatNumbers.Count == 1)
                    {
                        result.AppendLine($"{rowNumber}, место: {seatNumbers[0]}");
                    }
                    else
                    {
                        result.AppendLine($"{rowNumber}, места: {string.Join(", ", seatNumbers)}");
                    }
                }
            }

            selectedSeatsTextBox.Text = result.ToString();
        }

        private void UpdateTicketAmountTextBox()
        {
            txtBoxTicketAmount.Text = _ticketAmount.ToString();
        }
        #endregion

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            _frame.GoBack();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            var hall = LoadHallData();
            var rowsList = hall.Rows.ToList();

            if (btn != null)
            {
                int seatNumber = int.Parse(btn.Content.ToString());
                int rowIndex = Grid.GetRow(btn);

                if (rowIndex >= 0 && rowIndex < rowsList.Count)
                {
                    Row selectedRow = rowsList[rowIndex];

                    Seat selectedSeat = selectedRow.Seats.FirstOrDefault(seat => seat.SeatNumber == seatNumber.ToString());

                    if (selectedSeat != null)
                    {
                        if (btn.Style == (Style)FindResource("GreenArmchairButtonStyle"))
                        {
                            btn.Style = (Style)FindResource("PurpleArmchairButtonStyle");
                            _selectedPlaces.Add(selectedSeat);
                            _ticketAmount += _sessionDetails.selectedMovie.TicketPrice;
                        }
                        else if (btn.Style == (Style)FindResource("PurpleArmchairButtonStyle"))
                        {
                            btn.Style = (Style)FindResource("GreenArmchairButtonStyle");
                            _selectedPlaces.Remove(selectedSeat);
                            _ticketAmount -= _sessionDetails.selectedMovie.TicketPrice;
                        }

                        UpdateSelectedPlacesTextBox(_selectedPlaces, txtBoxSelected);
                        UpdateTicketAmountTextBox();
                    }
                }
            }
        }

        private void btnBuyingTicket_Click(object sender, RoutedEventArgs e)
        {
            string seats = txtBoxSelected.Text;
            string fullAmount = txtBoxTicketAmount.Text;

            var orderDetails = new OrderDetails
            {
                authorizedUser = _sessionDetails.authorizedUser,
                selectedMovie = _sessionDetails.selectedMovie,
                navigationFrame = _sessionDetails.navigationFrame,
                selectedShowing = _sessionDetails.selectedShowing,
                selectedPlaces = seats,
                fullAmount = Convert.ToDecimal(fullAmount)
            };

            PaymentWindow paymentWindow = new PaymentWindow(orderDetails);
            paymentWindow.ShowDialog();
        }
    }
}
