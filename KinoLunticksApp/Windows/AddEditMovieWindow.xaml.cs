using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Text.RegularExpressions;

using KinoLunticksApp.Models;
using KinoLunticksApp.Tools;

using Microsoft.EntityFrameworkCore;

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

        ImageWork _image = new ImageWork();

        static string _workingDirectory = Directory.GetParent(
                                    Directory.GetParent(
                                        Directory.GetParent(
                                            Environment.CurrentDirectory).
                                                            FullName).
                                                        FullName).
                                                       FullName;

        BitmapImage _currentImage,
                    _defaultImage = new BitmapImage(new Uri(_workingDirectory +
                                                            @"\Images\DefaultPhoto.png"));

        byte[] _imageData;

        public AddEditMovieWindow(Movie selectedMovie)
        {
            InitializeComponent();

            _movie = selectedMovie;

            _currentImage = _defaultImage;

            if (selectedMovie != null)
            {
                _currentMovie = selectedMovie;

                _imageData = _currentMovie.Preview;

                if (_currentMovie.Preview != null)
                {
                    _image.ReadBitmapImageFromArray(new MemoryStream(_currentMovie.Preview), out _currentImage);
                }

                txtBoxMovieCode.IsEnabled = false;
            }
            else
            {
                addEditWindow.Title = "Добавление фильма";
            }

            DataContext = _currentMovie;
        }

        private void imgPhoto_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BitmapImage photo = GetImage();

            if (photo != null)
            {
                imgPhoto.Source = photo;
            }
        }

        private void imgPhoto_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            imgPhoto.Source = _defaultImage;

            _imageData = null;
        }

        private void btnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            #region Код фильма
            if (string.IsNullOrWhiteSpace(_currentMovie.MovieCode.ToString()) ||
                string.IsNullOrEmpty(_currentMovie.MovieCode.ToString()))
            {
                errors.AppendLine("Введите корректный код фильма!");
            }
            else if (_db.Movies.Local.Select(movie => movie.MovieCode).ToList().
                        Contains(_currentMovie.MovieCode))
            {
                errors.AppendLine("Фильм с таким кодом уже существует!");
            }
            #endregion

            #region Название фильма
            if (string.IsNullOrWhiteSpace(_currentMovie.MovieName) ||
                string.IsNullOrEmpty(_currentMovie.MovieName))
            {
                errors.AppendLine("Введите корректное название фильма!");
            }
            #endregion

            #region Описание фильма
            if (string.IsNullOrWhiteSpace(_currentMovie.MovieDescription) ||
                string.IsNullOrEmpty(_currentMovie.MovieDescription))
            {
                errors.AppendLine("Введите корректное описание фильма!");
            }
            else if (!Regex.IsMatch(_currentMovie.MovieDescription, @"^[А-ЯЁ][а-яё]+"))
            {
                errors.AppendLine("Описание фильма должно начинаться с заглавной буквы!");
            }
            #endregion

            #region Рейтинг фильма
            if (string.IsNullOrWhiteSpace(_currentMovie.MovieRating.ToString()) ||
                string.IsNullOrEmpty(_currentMovie.MovieRating.ToString()))
            {
                errors.AppendLine("Введите корректный рейтинг фильма!");
            }
            #endregion

            #region Длительность фильма
            if (string.IsNullOrWhiteSpace(_currentMovie.MovieDuration) ||
                string.IsNullOrEmpty(_currentMovie.MovieDuration))
            {
                errors.AppendLine("Введите корректную длительность фильма!");
            }
            #endregion

            #region Возрастные ограничения
            if (string.IsNullOrWhiteSpace(_currentMovie.AgeRestriction) ||
                string.IsNullOrEmpty(_currentMovie.AgeRestriction))
            {
                errors.AppendLine("Введите корректные возрастные ограничения!");
            }
            #endregion

            #region Цена билета
            if (string.IsNullOrWhiteSpace(_currentMovie.TicketPrice.ToString()) ||
                string.IsNullOrEmpty(_currentMovie.TicketPrice.ToString()))
            {
                errors.AppendLine("Введите корректную цену билета!");
            }
            #endregion

            if (errors.Length > 0)
            {
                MessageBox.Show(
                    errors.ToString(),
                    "Заполните поля",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                    );

                return;
            }

            if (_movie != _currentMovie)
            {
                _db.Movies.Add(_currentMovie);
            }
            else
            {
                _db.Entry(_currentMovie).State = EntityState.Modified;
            }

            try
            {
                _currentMovie.Preview = _imageData;

                _db.SaveChanges();

                MessageBox.Show(
                    "Информация сохранена",
                    "Сохранение",
                     MessageBoxButton.OK,
                     MessageBoxImage.Information
                    );

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

        private void btnCancel_Click(object sender, RoutedEventArgs e) => Close();

        /// <summary>
        /// Получение изображения
        /// </summary>
        /// <returns>Загруженное изображение</returns>
        private BitmapImage GetImage()
        {
            _image.OpenImage(ref _imageData, ref _currentImage);

            return _currentImage;
        }
    }
}