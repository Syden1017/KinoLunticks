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

        public PaymentWindow(Movie selectedMovie, User user)
        {
            InitializeComponent();

            _movie = selectedMovie;
            _user = user;
        }
    }
}
