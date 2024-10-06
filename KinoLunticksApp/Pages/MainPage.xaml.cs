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

        // Поле фильтрации 
        const int FILTER_BY_MOVIE_GENRE = 1;         // Фильтр по жанру
        const int FILTER_BY_AGE_RESTRICTION = 2;     // Фильтр по возрастному ограничению

        // Поле сортировки  MovieRating
        const int SORT_BY_MOVIE_RATING = 0;          // Сортировка по рейтингу

        // Тип сортировки
        const int ASC_SORT = 0;                      // Сортировка по возрастанию

        public MainPage(Frame frame, User user)
        {
            InitializeComponent();

            _frame = frame;
            _user = user;

            UpdateMovieList();
        }

        private void UpdateMovieList()
        {
            _db.Movies.Load();
            _movies = _db.Movies.ToList();

            string request = txtBoxSearch.Text.
                                          Replace(" ", "").
                                          ToLower();
            int characteristics = 0;

            if (cmbBoxFilterType.SelectedIndex > 0)
            {
                int.TryParse(cmbBoxFilterType.SelectedValue.ToString(), out characteristics);
            }

            // Список формировать в порядке
            // сортировка -> поиск -> фильтрация -> деление на страницы
            int filterField = cmbBoxFilterField.SelectedIndex,
                sortField = cmbBoxSortField.SelectedIndex,
                sortType = cmbBoxSortType.SelectedIndex;

            List<Movie> movieList = SearchMovies(_movies, request);

            lViewLuntiki.ItemsSource = movieList;
        }

        #region Search
        /// <summary>
        /// Поиск студента по ФИО
        /// </summary>
        /// <param name="students">Список сеансов для поиска</param>
        /// <param name="request">Поисковый запрос</param>
        /// <returns>Результаты поиска</returns>
        private List<Movie> SearchMovies(List<Movie> movies, string request)
        {
            return movies.Where(s => (s.MovieName).
                                             ToLower().
                                             Contains(request)).
                                             ToList();
        }

        /// <summary>
        /// Изменение поискового запроса
        /// </summary>
        private void txtBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateMovieList();
        }
        #endregion

        /// <summary>
        /// Очистка данных из ComboBox
        /// </summary>
        /// <param name="comboBox">ComboBox для очистки</param>
        private void ClearComboBox(ComboBox comboBox)
        {
            try
            {
                comboBox.Items.Clear();
            }
            catch (Exception)
            {
                comboBox.ItemsSource = null;
            }
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
