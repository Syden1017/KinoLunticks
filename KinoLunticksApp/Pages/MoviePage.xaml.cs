using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

using KinoLunticksApp.Tools;
using KinoLunticksApp.Models;
using KinoLunticksApp.Windows;

using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using System;

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

            using(var db = new KinoLunticsContext())
            {
                var hallData = LoadHallData();
                PopulateHall(hallData);
            }

            DataContext = new
            {
                _sessionDetails.selectedMovie.Preview,
                _sessionDetails.selectedMovie.MovieName,
                _sessionDetails.selectedMovie.Genres,
                _sessionDetails.selectedMovie.AgeRestriction,
                _sessionDetails.selectedMovie.MovieDuration,
                hall = LoadHallData().HallNumber,
                formattedShowingDate = _sessionDetails.selectedShowing.ShowingDate.ToString("d MMMM", CultureInfo.CurrentCulture),
                _sessionDetails.authorizedUser,
                _sessionDetails.selectedShowing.ShowingTime
            };
        }

        #region Hall creating
        private Hall LoadHallData()
        {
            var hallId = _db.Showings.Where(s => s.ShowingDate == _sessionDetails.selectedShowing.ShowingDate &&
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
                        Content = seat.SeatNumber,
                        Style = (Style)FindResource("GreenArmchairButtonStyle")
                    };

                    seatButton.Click += Button_Click;

                    Grid.SetRow(seatButton, i);
                    Grid.SetColumn(seatButton, (i == rowsList.Count - 1) ? j + 1 : j + 2);

                    gHallSchema.Children.Add(seatButton);
                }
            }
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
