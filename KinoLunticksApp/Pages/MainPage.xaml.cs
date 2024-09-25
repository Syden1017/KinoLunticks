using KinoLunticksApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Windows.Controls;

namespace KinoLunticksApp.Pages
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        KinoLunticsContext _db = new KinoLunticsContext();
        User _user = new User();
        List<Movie> _movies = new List<Movie>();

        Frame _frame;

        public MainPage(Frame frame, User user)
        {
            InitializeComponent();

            _db.Movies.Load();
            _movies = _db.Movies.ToList();

            lViewLuntiki.ItemsSource = _movies;

            _frame = frame;
            _user = user;
        }

        private void txtBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void cmbBoxFilterField_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmbBoxFilterType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmbBoxSortField_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmbBoxSortType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnPersonalAccount_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _frame.Navigate(new PersonalAccountPage(_frame, _user));
        }
    }
}
