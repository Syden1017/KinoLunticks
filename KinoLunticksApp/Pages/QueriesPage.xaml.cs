using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using KinoLunticksApp.Models;
using KinoLunticksApp.Tools;

using Microsoft.EntityFrameworkCore;

namespace KinoLunticksApp.Pages
{
    /// <summary>
    /// Interaction logic for QueriesPage.xaml
    /// </summary>
    public partial class QueriesPage : Page
    {
        KinoLunticsContext _db = new KinoLunticsContext();

        Frame _frame;

        public QueriesPage(Frame frame)
        {
            InitializeComponent();

            _frame = frame;
        }

        /// <summary>
        /// Возвращает список фильмов по выбранному жанру
        /// </summary>
        /// <param name="genre">Жанр фильма</param>
        /// <returns>Результат запроса</returns>
        private List<Movie> GetMoviesByGenre(string genre)
        {
            return _db.Movies
                      .Include(m => m.Genres
                                     .Where(g => g.GenreName.ToLower().Contains(genre)))
                      .ToList();
        }

        /// <summary>
        /// Возвращает список 5 самых популярных фильмов
        /// </summary>
        /// <returns>Результат запроса</returns>
        private List<Movie> GetPopularMovies()
        {
            return _db.Movies
                      .Include(m => m.Showings)
                                     .ThenInclude(s => s.Orders)
                      .Where(m => m.Showings.Any(s => s.Orders.Any()))
                      .Select(m => new
                      {
                          Movie = m,
                          OrderCount = m.Showings.SelectMany(s => s.Orders).Count()
                      })
                      .OrderByDescending(m => m.OrderCount)
                      .Select(m => m.Movie)
                      .Take(5)
                      .ToList();
        }

        /// <summary>
        /// Возвращает список фильмов, которые идут в определенном зале
        /// </summary>
        /// <param name="hall">Номер зала</param>
        /// <returns>Результат запроса</returns>
        private List<Movie> GetMoviesByHall(int hall)
        {
            return _db.Showings
                      .Include(s => s.Movie)
                      .Where(s => s.HallId == hall)
                      .Select(s => s.Movie)
                      .Distinct()
                      .ToList();
        }

        /// <summary>
        /// Возвращает показы фильмов на сегодняшний день
        /// </summary>
        /// <returns>Результат запроса</returns>
        private List<Showing> GetShowingsForToday()
        {
            return _db.Showings
                      .Include(s => s.Movie)
                      .Where(s => s.ShowingDate == DateOnly.FromDateTime(DateTime.Today))
                      .ToList();
        }

        /// <summary>
        /// Возвращет показы, котороые уже идут на данный момент
        /// </summary>
        /// <returns>Результат запроса</returns>
        private List<Movie> GetOnGoingShowings()
        {
            var currentTime = TimeOnly.FromDateTime(DateTime.Now);

            return _db.Showings
                      .Include(s => s.Movie)
                      .Where(s => s.ShowingTime > currentTime)
                      .Select(s => s.Movie)
                      .Distinct()
                      .ToList();
        }

        private void btnQuery1_Click(object sender, RoutedEventArgs e)
        {
            dgQueries.ItemsSource = null;
            dgQueries.Columns.Clear();

            DataGridTextColumn id = new DataGridTextColumn();
            id.Header = "MovieCode";
            id.Binding = new Binding("MovieCode");

            DataGridTextColumn movieName = new DataGridTextColumn();
            movieName.Header = "MovieName";
            movieName.Binding = new Binding("MovieName");

            DataGridTextColumn movieDescription = new DataGridTextColumn();
            movieDescription.Header = "MovieDescription";
            movieDescription.Binding = new Binding("MovieDescription");

            DataGridTextColumn movieRating = new DataGridTextColumn();
            movieRating.Header = "MovieRating";
            movieRating.Binding = new Binding("MovieRating");

            DataGridTextColumn movieDuration = new DataGridTextColumn();
            movieDuration.Header = "MovieDuration";
            movieDuration.Binding = new Binding("MovieDuration");

            DataGridTextColumn producerName = new DataGridTextColumn();
            producerName.Header = "ProducerName";
            producerName.Binding = new Binding("ProducerName");

            DataGridTextColumn ageRestriction = new DataGridTextColumn();
            ageRestriction.Header = "AgeRestriction";
            ageRestriction.Binding = new Binding("AgeRestriction");

            dgQueries.Columns.Add(id);
            dgQueries.Columns.Add(movieName);
            dgQueries.Columns.Add(movieDescription);
            dgQueries.Columns.Add(movieRating);
            dgQueries.Columns.Add(movieDuration);
            dgQueries.Columns.Add(producerName);
            dgQueries.Columns.Add(ageRestriction);

            dgQueries.ItemsSource = GetPopularMovies();
        }

        private void btnQuery2_Click(object sender, RoutedEventArgs e)
        {
            dgQueries.ItemsSource = null;
            dgQueries.Columns.Clear();

            DataGridTextColumn id = new DataGridTextColumn();
            id.Header = "MovieCode";
            id.Binding = new Binding("MovieCode");

            DataGridTextColumn movieName = new DataGridTextColumn();
            movieName.Header = "MovieName";
            movieName.Binding = new Binding("MovieName");

            DataGridTextColumn movieDescription = new DataGridTextColumn();
            movieDescription.Header = "MovieDescription";
            movieDescription.Binding = new Binding("MovieDescription");

            DataGridTextColumn movieRating = new DataGridTextColumn();
            movieRating.Header = "MovieRating";
            movieRating.Binding = new Binding("MovieRating");

            DataGridTextColumn movieDuration = new DataGridTextColumn();
            movieDuration.Header = "MovieDuration";
            movieDuration.Binding = new Binding("MovieDuration");

            DataGridTextColumn producerName = new DataGridTextColumn();
            producerName.Header = "ProducerName";
            producerName.Binding = new Binding("ProducerName");

            DataGridTextColumn ageRestriction = new DataGridTextColumn();
            ageRestriction.Header = "AgeRestriction";
            ageRestriction.Binding = new Binding("AgeRestriction");

            dgQueries.Columns.Add(id);
            dgQueries.Columns.Add(movieName);
            dgQueries.Columns.Add(movieDescription);
            dgQueries.Columns.Add(movieRating);
            dgQueries.Columns.Add(movieDuration);
            dgQueries.Columns.Add(producerName);
            dgQueries.Columns.Add(ageRestriction);

            dgQueries.ItemsSource = GetMoviesByGenre(txtBoxGenre.Text);
        }

        private void btnQuery3_Click(object sender, RoutedEventArgs e)
        {
            string hallNumber = txtBoxHall.Text;
            int hallId = _db.Halls
                            .Where(h => h.HallNumber.ToLower().Contains(hallNumber.ToLower()))
                            .Select(h => h.HallId)
                            .FirstOrDefault();

            dgQueries.ItemsSource = null;
            dgQueries.Columns.Clear();

            DataGridTextColumn id = new DataGridTextColumn();
            id.Header = "MovieCode";
            id.Binding = new Binding("MovieCode");

            DataGridTextColumn movieName = new DataGridTextColumn();
            movieName.Header = "MovieName";
            movieName.Binding = new Binding("MovieName");

            DataGridTextColumn movieDescription = new DataGridTextColumn();
            movieDescription.Header = "MovieDescription";
            movieDescription.Binding = new Binding("MovieDescription");

            DataGridTextColumn movieRating = new DataGridTextColumn();
            movieRating.Header = "MovieRating";
            movieRating.Binding = new Binding("MovieRating");

            DataGridTextColumn movieDuration = new DataGridTextColumn();
            movieDuration.Header = "MovieDuration";
            movieDuration.Binding = new Binding("MovieDuration");

            DataGridTextColumn producerName = new DataGridTextColumn();
            producerName.Header = "ProducerName";
            producerName.Binding = new Binding("ProducerName");

            DataGridTextColumn ageRestriction = new DataGridTextColumn();
            ageRestriction.Header = "AgeRestriction";
            ageRestriction.Binding = new Binding("AgeRestriction");

            dgQueries.Columns.Add(id);
            dgQueries.Columns.Add(movieName);
            dgQueries.Columns.Add(movieDescription);
            dgQueries.Columns.Add(movieRating);
            dgQueries.Columns.Add(movieDuration);
            dgQueries.Columns.Add(producerName);
            dgQueries.Columns.Add(ageRestriction);

            dgQueries.ItemsSource = GetMoviesByHall(hallId);
        }

        private void btnQuery4_Click(object sender, RoutedEventArgs e)
        {
            dgQueries.ItemsSource = null;
            dgQueries.Columns.Clear();

            DataGridTextColumn id = new DataGridTextColumn();
            id.Header = "ShowingId";
            id.Binding = new Binding("ShowingId");

            DataGridTextColumn movieName = new DataGridTextColumn();
            movieName.Header = "Movie";
            movieName.Binding = new Binding("Movie.MovieName");

            DataGridTextColumn showingDate = new DataGridTextColumn();
            showingDate.Header = "ShowingDate";
            showingDate.Binding = new Binding("formattedShowingDate");

            DataGridTextColumn showingTime = new DataGridTextColumn();
            showingTime.Header = "ShowingTime";
            showingTime.Binding = new Binding("ShowingTime");

            dgQueries.Columns.Add(id);
            dgQueries.Columns.Add(movieName);
            dgQueries.Columns.Add(showingDate);
            dgQueries.Columns.Add(showingTime);

            dgQueries.ItemsSource = GetShowingsForToday();
        }

        private void btnQuery5_Click(object sender, RoutedEventArgs e)
        {
            dgQueries.ItemsSource = null;
            dgQueries.Columns.Clear();

            DataGridTextColumn id = new DataGridTextColumn();
            id.Header = "MovieCode";
            id.Binding = new Binding("MovieCode");

            DataGridTextColumn movieName = new DataGridTextColumn();
            movieName.Header = "MovieName";
            movieName.Binding = new Binding("MovieName");

            DataGridTextColumn movieDescription = new DataGridTextColumn();
            movieDescription.Header = "MovieDescription";
            movieDescription.Binding = new Binding("MovieDescription");

            DataGridTextColumn movieRating = new DataGridTextColumn();
            movieRating.Header = "MovieRating";
            movieRating.Binding = new Binding("MovieRating");

            DataGridTextColumn movieDuration = new DataGridTextColumn();
            movieDuration.Header = "MovieDuration";
            movieDuration.Binding = new Binding("MovieDuration");

            DataGridTextColumn producerName = new DataGridTextColumn();
            producerName.Header = "ProducerName";
            producerName.Binding = new Binding("ProducerName");

            DataGridTextColumn ageRestriction = new DataGridTextColumn();
            ageRestriction.Header = "AgeRestriction";
            ageRestriction.Binding = new Binding("AgeRestriction");

            dgQueries.Columns.Add(id);
            dgQueries.Columns.Add(movieName);
            dgQueries.Columns.Add(movieDescription);
            dgQueries.Columns.Add(movieRating);
            dgQueries.Columns.Add(movieDuration);
            dgQueries.Columns.Add(producerName);
            dgQueries.Columns.Add(ageRestriction);

            dgQueries.ItemsSource = GetOnGoingShowings();
        }
    }
}
