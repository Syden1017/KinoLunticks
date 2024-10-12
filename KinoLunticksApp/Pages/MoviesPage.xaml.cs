using System.Windows.Controls;

using KinoLunticksApp.Models;

namespace KinoLunticksApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для MoviesPage.xaml
    /// </summary>
    public partial class MoviesPage : Page
    {
        User _user = new User();
        Movie _movie = new Movie();

        Frame _frame;

        public MoviesPage(Frame frame, User user, Movie movie)
        {
            InitializeComponent();

            _frame = frame;
            _user = user;
            _movie = movie;

            DataContext = _user;
            DataContext = _movie;
        }
    }
}