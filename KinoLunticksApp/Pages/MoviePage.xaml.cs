using System.Windows;
using System.Windows.Controls;

using KinoLunticksApp.Models;
using KinoLunticksApp.Windows;

namespace KinoLunticksApp.Pages
{
    /// <summary>
    /// Interaction logic for MoviePage.xaml
    /// </summary>
    public partial class MoviePage : Page
    {
        User _user = new User();
        Movie _movie = new Movie();

        Frame _frame;

        private bool[,] _hallLayout = new bool[3, 10];
        private List<Button> _armchairButtons;
        private List<string> _selectedPlaces;
        private decimal _ticketAmount;

        public MoviePage(Frame frame, User user, Movie movie)
        {
            InitializeComponent();

            _frame = frame;
            _user = user;
            _movie = movie;

            _armchairButtons = new List<Button>
            {
                btn1Place1, btn1Place2, btn1Place3, btn1Place4, btn1Place5, btn1Place6, btn1Place7, btn1Place8, btn1Place9,
                btn2Place1, btn2Place2, btn2Place3, btn2Place4, btn2Place5, btn2Place6, btn2Place7, btn2Place8, btn2Place9,
                btn3Place1, btn3Place2, btn3Place3, btn3Place4, btn3Place5, btn3Place6, btn3Place7, btn3Place8, btn3Place9, btn3Place10
            };
            _selectedPlaces = new List<string>();
            _ticketAmount = 0;

            UpdateSelectedPlacesTextBox();
            UpdateTicketAmountTextBox();

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    _hallLayout[i, j] = false;
                }
            }

            DataContext = _movie;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            _frame.GoBack();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            if (btn.Style == (Style)FindResource("GreenArmchairButtonStyle"))
            {
                btn.Style = (Style)FindResource("PurpleArmchairButtonStyle");
                _selectedPlaces.Add(btn.Content.ToString());
                _ticketAmount += _movie.TicketPrice;
            }
            else if (btn.Style == (Style)FindResource("PurpleArmchairButtonStyle"))
            {
                btn.Style = (Style)FindResource("GreenArmchairButtonStyle");
                _selectedPlaces.Remove(btn.Content.ToString());
                _ticketAmount -= _movie.TicketPrice;
            }

            UpdateSelectedPlacesTextBox();
            UpdateTicketAmountTextBox();
        }

        private void UpdateSelectedPlacesTextBox()
        {
            txtBoxSelected.Text = string.Join(", ", _selectedPlaces);
        }

        private void UpdateTicketAmountTextBox()
        {
            txtBoxTicketAmount.Text = _ticketAmount.ToString();
        }

        private void btnBuyingTicket_Click(object sender, RoutedEventArgs e)
        {
            PaymentWindow paymentWindow = new PaymentWindow(_movie, _user);
            paymentWindow.ShowDialog();
        }

        private void btn1Place1_Click(object sender, RoutedEventArgs e)
        {
            Button_Click(sender, e);
            ToggleSeat(0, 0);
        }

        private void btn1Place2_Click(object sender, RoutedEventArgs e)
        {
            Button_Click(sender, e);
            ToggleSeat(0, 1);
        }

        private void btn1Place3_Click(object sender, RoutedEventArgs e)
        {
            Button_Click(sender, e);
            ToggleSeat(0, 2);
        }

        private void btn1Place4_Click(object sender, RoutedEventArgs e)
        {
            Button_Click(sender, e);
            ToggleSeat(0, 3);
        }

        private void btn1Place5_Click(object sender, RoutedEventArgs e)
        {
            Button_Click(sender, e);
            ToggleSeat(0, 4);
        }

        private void btn1Place6_Click(object sender, RoutedEventArgs e)
        {
            Button_Click(sender, e);
            ToggleSeat(0, 5);
        }

        private void btn1Place7_Click(object sender, RoutedEventArgs e)
        {
            Button_Click(sender, e);
            ToggleSeat(0, 6);
        }

        private void btn1Place8_Click(object sender, RoutedEventArgs e)
        {
            Button_Click(sender, e);
            ToggleSeat(0, 7);
        }

        private void btn1Place9_Click(object sender, RoutedEventArgs e)
        {
            Button_Click(sender, e);
            ToggleSeat(0, 8);
        }

        private void btn2Place1_Click(object sender, RoutedEventArgs e)
        {
            Button_Click(sender, e);
            ToggleSeat(1, 0);
        }

        private void btn2Place2_Click(object sender, RoutedEventArgs e)
        {
            Button_Click(sender, e);
            ToggleSeat(1, 1);
        }

        private void btn2Place3_Click(object sender, RoutedEventArgs e)
        {
            Button_Click(sender, e);
            ToggleSeat(1, 2);
        }

        private void btn2Place4_Click(object sender, RoutedEventArgs e)
        {
            Button_Click(sender, e);
            ToggleSeat(1, 3);
        }

        private void btn2Place5_Click(object sender, RoutedEventArgs e)
        {
            Button_Click(sender, e);
            ToggleSeat(1, 4);
        }

        private void btn2Place6_Click(object sender, RoutedEventArgs e)
        {
            Button_Click(sender, e);
            ToggleSeat(1, 5);
        }

        private void btn2Place7_Click(object sender, RoutedEventArgs e)
        {
            Button_Click(sender, e);
            ToggleSeat(1, 6);
        }

        private void btn2Place8_Click(object sender, RoutedEventArgs e)
        {
            Button_Click(sender, e);
            ToggleSeat(1, 7);
        }

        private void btn2Place9_Click(object sender, RoutedEventArgs e)
        {
            Button_Click(sender, e);
            ToggleSeat(1, 8);
        }

        private void btn3Place1_Click(object sender, RoutedEventArgs e)
        {
            Button_Click(sender, e);
            ToggleSeat(2, 0);
        }

        private void btn3Place2_Click(object sender, RoutedEventArgs e)
        {
            Button_Click(sender, e);
            ToggleSeat(2, 1);
        }

        private void btn3Place3_Click(object sender, RoutedEventArgs e)
        {
            Button_Click(sender, e);
            ToggleSeat(2, 2);
        }

        private void btn3Place4_Click(object sender, RoutedEventArgs e)
        {
            Button_Click(sender, e);
            ToggleSeat(2, 3);
        }

        private void btn3Place5_Click(object sender, RoutedEventArgs e)
        {
            Button_Click(sender, e);
            ToggleSeat(2, 4);
        }

        private void btn3Place6_Click(object sender, RoutedEventArgs e)
        {
            Button_Click(sender, e);
            ToggleSeat(2, 5);
        }

        private void btn3Place7_Click(object sender, RoutedEventArgs e)
        {
            Button_Click(sender, e);
            ToggleSeat(2, 6);
        }

        private void btn3Place8_Click(object sender, RoutedEventArgs e)
        {
            Button_Click(sender, e);
            ToggleSeat(2, 7);
        }

        private void btn3Place9_Click(object sender, RoutedEventArgs e)
        {
            Button_Click(sender, e);
            ToggleSeat(2, 8);
        }

        private void btn3Place10_Click(object sender, RoutedEventArgs e)
        {
            Button_Click(sender, e);
            ToggleSeat(2, 9);
        }

        private void ToggleSeat(int row, int col)
        {
            _hallLayout[row, col] = !_hallLayout[row, col];
            UpdateSelectedSeatsTextBox();
        }

        private void UpdateSelectedSeatsTextBox()
        {
            List<string> selectedSeats = new List<string>();

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (_hallLayout[i, j])
                    {
                        selectedSeats.Add($"Ряд {i + 1}, Место {j + 1}");
                    }
                }
            }

            txtBoxSelected.Text = string.Join("; ", selectedSeats);
        }
    }
}
