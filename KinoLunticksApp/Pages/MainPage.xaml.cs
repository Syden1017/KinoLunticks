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
                int.TryParse(cmbBoxFilterType.SelectedIndex.ToString(), out characteristics);
            }

            // Список формировать в порядке
            // сортировка -> поиск -> фильтрация -> деление на страницы
            int filterField = cmbBoxFilterField.SelectedIndex,
                sortField = cmbBoxSortField.SelectedIndex,
                sortType = cmbBoxSortType.SelectedIndex;

            List<Movie> movieList = SearchMovies(
                                        FilterMovies(_movies,
                                                     filterField,
                                                     characteristics),
                                                 request);

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

        #region Filter
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

        /// <summary>
        /// <para>Получение списка годов рождения студентов</para>
        /// <para>Загрузка списка в ComboBox</para>
        /// </summary>
        /// <param name="filterType">ComboBox для загрузки</param>
        private void LoadGenresInComboBox(ComboBox filterType)
        {
            List<string> genres = _db.Movies.Select(m => m.MovieGenre.ToString()).
                                                                      Distinct().
                                                                      OrderBy(y => y).
                                                                      ToList();

            genres.Insert(0, "Все жанры");

            ClearComboBox(filterType);

            filterType.ItemsSource = genres;
            filterType.SelectedIndex = 0;
        }

        /// <summary>
        /// <para>Получение списка годов рождения студентов</para>
        /// <para>Загрузка списка в ComboBox</para>
        /// </summary>
        /// <param name="filterType">ComboBox для загрузки</param>
        private void LoadAgeRestrictionsInComboBox(ComboBox filterType)
        {
            List<string> ageRestriction = _db.Movies.Select(m => m.AgeRestriction.ToString()).
                                                                      Distinct().
                                                                      OrderBy(y => y).
                                                                      ToList();

            ageRestriction.Insert(0, "Все ограничения");

            ClearComboBox(filterType);

            filterType.ItemsSource = ageRestriction;
            filterType.SelectedIndex = 0;
        }

        /// <summary>
        /// Фильтрация списка студентов по году поступления / году рождения
        /// </summary>
        /// <param name="movies">Список студентов для фильтрации</param>
        /// <param name="filterField">Номер поля для фильтрации</param>
        /// <param name="characteristics">Значение года фильтрации</param>
        /// <returns>Результаты фильтрации</returns>
        private List<Movie> FilterMovies(List<Movie> movies, int filterField, int characteristics)
        {
            if (characteristics != 0)
            {
                switch (filterField)
                {
                    case FILTER_BY_MOVIE_GENRE:
                        movies = movies.Where(m => m.MovieGenre.ToLower().Contains(characteristics.ToString().ToLower())).ToList();

                        break;

                    case FILTER_BY_AGE_RESTRICTION:
                        movies = movies.Where(m => m.AgeRestriction.Contains(characteristics.ToString())).ToList();

                        break;

                    default:
                        break;
                }
            }

            return movies;
        }

        private void cmbBoxFilterField_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (cmbBoxFilterField.SelectedIndex)
            {
                // Фильтр по жанру
                case FILTER_BY_MOVIE_GENRE:
                    LoadGenresInComboBox(cmbBoxFilterType);
                    break;

                // Фильтр по возрастным ограничениям
                case FILTER_BY_AGE_RESTRICTION:
                    LoadAgeRestrictionsInComboBox(cmbBoxFilterType);
                    break;

                // Поле фильтра не выбрано
                default:
                    ClearComboBox(cmbBoxFilterType);
                    cmbBoxFilterType.Items.Add("Не задано");
                    cmbBoxFilterType.SelectedIndex = 0;
                    break;
            }
        }

        private void cmbBoxFilterType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateMovieList();
        }
        #endregion

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
