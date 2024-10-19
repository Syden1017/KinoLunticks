using System.Windows;

using KinoLunticksApp.Models;

namespace KinoLunticksApp.Windows
{
    /// <summary>
    /// Interaction logic for PaymentWindow.xaml
    /// </summary>
    public partial class PaymentWindow : Window
    {
        User _user = new User();
        Movie _movie = new Movie();

        string _seats;
        string _fullAmount;

        public PaymentWindow(Movie selectedMovie, User user, string seats, string fullAmount)
        {
            InitializeComponent();

            _movie = selectedMovie;
            _user = user;
            _seats = seats;
            _fullAmount = fullAmount;

            DataContext = new
            {
                moviePreview = _movie.Preview,
                movieName = _movie.MovieName,
                movieAgeRestriction = _movie.AgeRestriction,
                movieDuration = _movie.MovieDuration,
                seats = _seats,
                fullAmount = _fullAmount
            };
        }

        private void btnPay_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {

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
    }
}