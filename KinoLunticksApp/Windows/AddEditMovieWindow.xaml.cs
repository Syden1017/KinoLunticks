using System.IO;
using System.Windows;

using KinoLunticksApp.Models;

namespace KinoLunticksApp.Windows
{
    /// <summary>
    /// Interaction logic for AddEditMovieWindow.xaml
    /// </summary>
    public partial class AddEditMovieWindow : Window
    {
        KinoLunticsContext _db = new KinoLunticsContext();
        Movie _currentMovie = new Movie(),
              _movie;

        public AddEditMovieWindow(Movie selectedMovie)
        {
            InitializeComponent();

            _movie = selectedMovie;

            if (selectedMovie != null)
            {
                _currentMovie = selectedMovie;
            }
            else
            {

            }

            DataContext = _currentMovie;
        }
    }
}
