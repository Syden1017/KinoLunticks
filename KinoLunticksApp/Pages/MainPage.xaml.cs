using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

using KinoLunticksApp.Models;

using Microsoft.EntityFrameworkCore;

namespace KinoLunticksApp.Pages
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        // Поле фильтрации 
        const int FILTER_BY_MOVIE_GENRE = 1;         // Фильтр по жанру
        const int FILTER_BY_AGE_RESTRICTION = 2;     // Фильтр по возрастному ограничению

        // Поле сортировки  MovieRating
        const int SORT_BY_MOVIE_RATING      = 0,    // Сортировка по рейтингу
                  SORT_BY_MOVIE_NAME        = 1,    // Сортировка по названию
                  SORT_BY_MOVIE_DURATION    = 2,    // Сортировка по длительности
                  SORT_BY_AGE_RESTRICTIONS  = 3,    // Сортировка по возрастным ограничениям
                  SORT_BY_TICKET_PRICE      = 4;    // Сортировка по цене билета

        // Тип сортировки
        const int ASC_SORT = 0;                      // Сортировка по возрастанию

        KinoLunticsContext _db = new KinoLunticsContext();
        User _user = new User();
        List<Movie> _movies = new List<Movie>();

        Frame _frame;

        int _movieCount = 0;

        // Распределение по страницам
        int _currentPage = 1,
            _maxPage = 0;

        public MainPage(Frame frame, User user)
        {
            InitializeComponent();

            _frame = frame;
            _user = user;

            cmbBoxFilterField.SelectedIndex = 0;
            cmbBoxFilterType.SelectedIndex = 0;
            cmbBoxSortField.SelectedIndex = 0;
            cmbBoxSortType.SelectedIndex = 0;
            cmbBoxMovieCount.SelectedIndex = 0;

            UpdateMovieList();
        }

        private void UpdateMovieList()
        {
            _db.Movies.Include(m => m.Genres).Load();
            _movies = _db.Movies.ToList();
            _movieCount = _movies.Count;

            string request = txtBoxSearch.Text.
                                          Replace(" ", "").
                                          ToLower();

            int characteristics = 0;

            if (cmbBoxFilterType.SelectedIndex > 0)
            {
                int.TryParse(cmbBoxFilterType.SelectedIndex.ToString(), out characteristics);
            }

            // Список формировать в порядке
            // сортировка -> поиск -> фильтрация
            int filterField = cmbBoxFilterField.SelectedIndex,
                sortField = cmbBoxSortField.SelectedIndex,
                sortType = cmbBoxSortType.SelectedIndex;

            List<Movie> movieList = SortMovies(
                                        SearchMovies(
                                            FilterMovies(
                                                GetPages(_movies),
                                                        filterField,
                                                        characteristics),
                                                     request),
                                               sortField,
                                               sortType);

            txtBoxCurrentPage.Text = _currentPage.ToString();
            txtBoxCurrentPage.MaxLength = _maxPage.ToString().Length;
            txtBoxTotalPage.Text = _maxPage.ToString();

            lViewLuntiki.ItemsSource = movieList;

            txtBlockMovieCount.Text = _movieCount.ToString();
        }

        #region Search
        /// <summary>
        /// Поиск фильма по названию
        /// </summary>
        /// <param name="movies"> Список сеансов для поиска</param>
        /// <param name="request"> Поисковый запрос</param>
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
        /// <para>Получение списка жанров фильмов</para>
        /// <para>Загрузка списка в ComboBox</para>
        /// </summary>
        /// <param name="filterType">ComboBox для загрузки</param>
        private void LoadGenresInComboBox(ComboBox filterType)
        {
            List<string> genres = _db.Genres.Select(g => g.GenreName.ToString()).
                                                                     ToList();

            genres.Insert(0, "Все жанры");

            ClearComboBox(filterType);

            filterType.ItemsSource = genres;
            filterType.SelectedIndex = 0;
        }

        /// <summary>
        /// <para>Получение списка возрастных ограничений фильмов</para>
        /// <para>Загрузка списка в ComboBox</para>
        /// </summary>
        /// <param name="filterType">ComboBox для загрузки</param>
        private void LoadAgeRestrictionsInComboBox(ComboBox filterType)
        {
            List<string> ageRestriction = _db.Movies.Select(m => m.AgeRestriction.ToString()).
                                                                                  Distinct().
                                                                                  ToList();

            ageRestriction.Insert(0, "Все ограничения");

            ClearComboBox(filterType);

            filterType.ItemsSource = ageRestriction;
            filterType.SelectedIndex = 0;
        }

        /// <summary>
        /// Фильтрация списка фильмов по жанрам / возрастным ограничениям
        /// </summary>
        /// <param name="movies">Список фильмов для фильтрации</param>
        /// <param name="filterField">Номер поля для фильтрации</param>
        /// <param name="characteristics">Значение фильтрации</param>
        /// <returns>Результаты фильтрации</returns>
        private List<Movie> FilterMovies(List<Movie> movies, int filterField, int characteristics)
        {
            if (characteristics != 0)
            {
                switch (filterField)
                {
                    case FILTER_BY_AGE_RESTRICTION:
                        movies = movies.Where(m => m.AgeRestriction == cmbBoxFilterType.SelectedValue.ToString()).ToList();
                        break;

                    case FILTER_BY_MOVIE_GENRE:
                        movies = movies.Where(m => m.Genres.Any(g => g.GenreName == cmbBoxFilterType.SelectedValue.ToString())).ToList();
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

        #region Sort
        /// <summary>
        /// Сортировка списка студентов
        /// </summary>
        /// <param name="movies">Список фильмов для сортировки</param>
        /// <param name="sortField">Номер поля сортировки</param>
        /// <param name="sortType">Номер типа сортировки</param>
        /// <returns>Отсортированный список</returns>
        private List<Movie> SortMovies(List<Movie> movies, int sortField, int sortType)
        {
            Func<Movie, object> sortExpression;

            switch (sortField)
            {
                case SORT_BY_MOVIE_RATING:
                    sortExpression = m => m.MovieRating;
                    break;

                case SORT_BY_MOVIE_NAME:
                    sortExpression = m => m.MovieName;
                    break;

                case SORT_BY_MOVIE_DURATION:
                    sortExpression = m => m.MovieDuration;
                    break;

                case SORT_BY_AGE_RESTRICTIONS:
                    sortExpression = m => m.AgeRestriction;
                    break;

                case SORT_BY_TICKET_PRICE:
                    sortExpression = m => m.TicketPrice;
                    break;

                default:
                    sortExpression = m => m.MovieRating;
                    break;
            }

            return sortType == ASC_SORT ? movies.OrderBy(sortExpression).ToList() :
                                          movies.OrderByDescending(sortExpression).ToList();
        }

        private void cmbBoxSortField_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateMovieList();
        }

        private void cmbBoxSortType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateMovieList();
        }
        #endregion

        #region Pages
        /// <summary>
        /// Деление списка фильмов на страницы
        /// </summary>
        /// <param name="movies">Список фильмов</param>
        /// <returns>Страница из списка фильмов</returns>
        private List<Movie> GetPages(List<Movie> movies)
        {
            int pageSize = _movieCount;

            if (cmbBoxMovieCount.SelectedIndex != 0)
            {
                pageSize = cmbBoxMovieCount.SelectedIndex * 4;
            }

            _maxPage = (int)Math.Ceiling(
                       (double)_db.Movies.Count() / pageSize);

            return movies.Skip((_currentPage - 1) * pageSize).
                          Take(pageSize).
                          ToList();
        }

        /// <summary>
        /// Переход на заданную страницу
        /// </summary>
        /// <param name="page">Номер страницы</param>
        private void GoToPage(int page)
        {
            _currentPage = page;

            UpdateMovieList();
        }

        private void cmbBoxMovieCount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GoToPage(1);
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            GoToPage(1);
        }

        private void btnEnd_Click(object sender, RoutedEventArgs e)
        {
            GoToPage(_maxPage);
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage - 1 < 1)
            {
                return;
            }

            GoToPage(--_currentPage);
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage + 1 > _maxPage)
            {
                return;
            }

            GoToPage(++_currentPage);
        }

        private void txtBoxCurrentPage_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_currentPage >= 1 &&
                _currentPage <= _maxPage &&
                txtBoxCurrentPage.Text != string.Empty)
            {
                if (!int.TryParse(txtBoxCurrentPage.Text, out _currentPage))
                {
                    _currentPage = 1;

                    MessageBox.Show(
                        "Номер страницы введён некорректно.\n" +
                        "Осуществлено перенаправление в начало списка.",
                        "Внимание!",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                        );
                }
                else if (_currentPage > _maxPage)
                {
                    _currentPage = _maxPage;

                    MessageBox.Show(
                        "Номер страницы не найден.\n" +
                        "Осуществлено перенаправление в конец списка.",
                        "Внимание!",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                        );
                }
            }

            GoToPage(_currentPage);
        }
        #endregion

        private void btnPersonalAccount_Click(object sender, RoutedEventArgs e) => _frame.Navigate(new PersonalAccountPage(_frame, _user));

        private void lViewLuntiki_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedMovie = lViewLuntiki.SelectedItem as Movie;

            if (selectedMovie != null)
            {
                _frame.Navigate(new FullInfoPage(_frame, _user, selectedMovie));
            }
        }
    }
}