using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Globalization;
using System.Windows.Controls;

using KinoLunticksApp.Tools;
using KinoLunticksApp.Models;
using System.Diagnostics;

namespace KinoLunticksApp.Pages
{
    /// <summary>
    /// Interaction logic for FullInfoPage.xaml
    /// </summary>
    public partial class FullInfoPage : Page
    {
        KinoLunticsContext _db = new KinoLunticsContext();
        Movie _movie = new Movie();
        User _user = new User();

        Frame _frame;
        Button _previousButton;

        private static readonly char[] trimCharsArray = ['{', '}', ' '];

        public FullInfoPage(Frame frame, User user, Movie movie)
        {
            InitializeComponent();

            _frame = frame;
            _movie = movie;
            _user = user;

            DataContext = _movie;
            LoadShowings(_movie.MovieCode);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            _frame.GoBack();
        }

        private void btnDate_Click(object sender, RoutedEventArgs e)
        {
            var selectedDate = (DateOnly)((Button)sender).Tag;

            LoadShowingsByDate(selectedDate);

            HighlightSelectedDate((Button)sender);
        }

        private void btnShowingTime_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            string tagValue = button.Tag.ToString();
            var defaultDate = (DateOnly)((Button)stcPanelDates.Children[0]).Tag;

            Showing selectedShowing = GetShowingById(tagValue, _movie.MovieCode, defaultDate);

            var sessionDetails = new SessionDetails
            {
                authorizedUser = _user,
                selectedMovie = _movie,
                navigationFrame = _frame,
                selectedShowing = selectedShowing
            };

            _frame.Navigate(new MoviePage(sessionDetails));
        }

        private Showing GetShowingById(string tagValue, int movieId, DateOnly date)
        {
            tagValue = tagValue.Trim(trimCharsArray);

            string[] parts = tagValue.Split(',');

            string showingTimePart = parts.FirstOrDefault(part => part.Trim().StartsWith("ShowingTime"));

            if (showingTimePart != null)
            {
                string showingTimeString = showingTimePart.Substring(showingTimePart.IndexOf('=') + 1).Trim();

                try
                {
                    TimeOnly showingTime = TimeOnly.Parse(showingTimeString);

                    return _db.Showings.FirstOrDefault(s =>
                        s.MovieId == movieId &&
                        s.ShowingDate == date &&
                        s.ShowingTime == showingTime);
                }
                catch (FormatException ex)
                {
                    Debug.WriteLine($"Error parsing showing time: {ex.Message}");
                    return null;
                }
            }

            return null;
        }

        #region Methods for creating user interface
        /// <summary>
        /// Загружает показы по выбранному фильму
        /// </summary>
        /// <param name="movieId">Код фильма</param>
        private void LoadShowings(int movieId)
        {
            try
            {
                var currentDate = DateOnly.FromDateTime(DateTime.Now);
                var showings = _db.Showings.Where(s => s.Movie.MovieCode == movieId).ToList();
                var dates = showings.Select(s => s.ShowingDate).Distinct().OrderBy(d => d).ToList();

                stcPanelDates.Children.Clear();

                foreach (var date in dates)
                {
                    if (date >= currentDate)
                    {
                        AddDateButton(date);
                    }
                }

                var defaultDate = (DateOnly)((Button)stcPanelDates.Children[0]).Tag;
                LoadShowingsByDate(defaultDate);
                HighlightSelectedDate((Button)stcPanelDates.Children[0]);
            }
            catch (Exception ex)
            {
                AddTextBlock();
            }
        }

        /// <summary>
        /// Добавляет на форму кнопку
        /// </summary>
        /// <param name="date">Дата для отображения в качестве контента кнопки</param>
        private void AddDateButton(DateOnly date)
        {
            var button = new Button
            {
                Content = $"{date.ToString("d MMMM", CultureInfo.CurrentCulture)}",
                Tag = date,
                Background = new SolidColorBrush(Colors.Transparent),
                BorderBrush = new SolidColorBrush(Color.FromRgb(106, 106, 106)),
                BorderThickness = new Thickness(1),
                Foreground = new SolidColorBrush(Colors.White),
                Margin = new Thickness(7),
                Padding = new Thickness(15),
                Cursor = Cursors.Hand,
                FontSize = 18
            };

            ControlTemplate template = new ControlTemplate(typeof(Button));

            FrameworkElementFactory border = new FrameworkElementFactory(typeof(Border));
            border.SetValue(Border.CornerRadiusProperty, new CornerRadius(7));
            border.SetValue(Border.BorderBrushProperty, new SolidColorBrush(Color.FromRgb(106, 106, 106)));
            border.SetValue(Border.BorderThicknessProperty, new Thickness(1.2));
            border.SetValue(Border.PaddingProperty, new Thickness(10, 7, 10, 7));

            FrameworkElementFactory contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            contentPresenter.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);

            border.AppendChild(contentPresenter);
            template.VisualTree = border;

            button.Template = template;

            button.Click += btnDate_Click;
            stcPanelDates.Children.Add(button);
        }

        /// <summary>
        /// Перекрашывает границы у выбранной кнопки
        /// </summary>
        /// <param name="selectedButton">Выбранная кнопка с датой</param>
        private void HighlightSelectedDate(Button selectedButton)
        {
            if (selectedButton == null)
            {
                throw new ArgumentNullException(nameof(selectedButton), "Выбранная кнопка не может быть NULL");
            }

            if(_previousButton != null)
            {
                ResetButtonStyle(_previousButton);
            }

            ControlTemplate template = new ControlTemplate(typeof(Button));

            FrameworkElementFactory border = new FrameworkElementFactory(typeof(Border));
            border.SetValue(Border.CornerRadiusProperty, new CornerRadius(7));
            border.SetValue(Border.BorderBrushProperty, new SolidColorBrush(Color.FromRgb(72, 155, 160)));
            border.SetValue(Border.BorderThicknessProperty, new Thickness(1.2));
            border.SetValue(Border.PaddingProperty, new Thickness(10, 7, 10, 7));

            FrameworkElementFactory contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            contentPresenter.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);

            border.AppendChild(contentPresenter);
            template.VisualTree = border;

            selectedButton.Template = template;

            _previousButton = selectedButton;
        }

        /// <summary>
        /// Загружает времена сеансов по выбранной дате
        /// </summary>
        /// <param name="selectedDate">Выбранная дата</param>
        private void LoadShowingsByDate(DateOnly selectedDate)
        {
            var showings = _db.Showings.
                Where(s => s.MovieId == _movie.MovieCode && s.ShowingDate == selectedDate).
                Select(s => new
                {
                    s.ShowingTime,
                    s.Movie.TicketPrice
                }).
                ToList();

            stcPanelShowingsTimes.Children.Clear();

            foreach (var showing in showings)
            {
                var button = new Button
                {
                    Content = $"{showing.ShowingTime}\n{showing.TicketPrice:C}",
                    Tag = showing,
                    Background = new SolidColorBrush(Colors.Transparent),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(106, 106, 106)),
                    BorderThickness = new Thickness(1),
                    Foreground = new SolidColorBrush(Colors.White),
                    Margin = new Thickness(15, 0, 10, 0),
                    Cursor = Cursors.Hand,
                    Height = 75,
                    FontSize = 18,
                    HorizontalContentAlignment = HorizontalAlignment.Center
                };

                ControlTemplate template = new ControlTemplate(typeof(Button));

                FrameworkElementFactory border = new FrameworkElementFactory(typeof(Border));
                border.SetValue(Border.CornerRadiusProperty, new CornerRadius(10));
                border.SetValue(Border.BorderBrushProperty, new SolidColorBrush(Color.FromRgb(106, 106, 106)));
                border.SetValue(Border.BorderThicknessProperty, new Thickness(1));
                border.SetValue(Border.PaddingProperty, new Thickness(10, 7, 10, 7));

                FrameworkElementFactory contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
                contentPresenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                contentPresenter.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);

                border.AppendChild(contentPresenter);
                template.VisualTree = border;
                button.Template = template;

                button.Click += btnShowingTime_Click;
                stcPanelShowingsTimes.Children.Add(button);
            }
        }

        /// <summary>
        /// Сбрасывает стили у кнопки
        /// </summary>
        /// <param name="button">Кнопка для сброса стилей</param>
        private void ResetButtonStyle(Button button)
        {
            ControlTemplate defaultTemplate = new ControlTemplate(typeof(Button));

            FrameworkElementFactory defaultBorder = new FrameworkElementFactory(typeof(Border));
            defaultBorder.SetValue(Border.CornerRadiusProperty, new CornerRadius(7));
            defaultBorder.SetValue(Border.BorderBrushProperty, new SolidColorBrush(Color.FromRgb(106, 106, 106)));
            defaultBorder.SetValue(Border.BorderThicknessProperty, new Thickness(1));
            defaultBorder.SetValue(Border.PaddingProperty, new Thickness(10, 7, 10, 7));

            FrameworkElementFactory defaultContentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
            defaultContentPresenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            defaultContentPresenter.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);

            defaultBorder.AppendChild(defaultContentPresenter);
            defaultTemplate.VisualTree = defaultBorder;

            button.Template = defaultTemplate;
        }
        
        /// <summary>
        /// Добавляет на форму текстовую метку
        /// </summary>
        private void AddTextBlock()
        {
            var textBlock = new TextBlock
            {
                Text = "У данного фильма нет сеансов",
                Background = new SolidColorBrush(Colors.Transparent),
                Foreground = new SolidColorBrush(Colors.White),
                FontSize = 18
            };

            stcPanelDates.Children.Add(textBlock);
        }
        #endregion
    }
}