using System.Windows;

using KinoLunticksApp.Models;

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
            TimeOnly sessionTime = TimeOnly.Parse(txtBlockSessionTime.Text);

            var order = new Order
            {
                User = _user.Login,
                Movie = _movie.MovieCode,
                SessionTime = sessionTime,
                Seats = _seats,
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
    }
}