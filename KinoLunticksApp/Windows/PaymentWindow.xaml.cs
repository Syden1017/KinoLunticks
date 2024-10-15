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
        }
    }
}
