using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Globalization;
using System.Windows.Controls;

using KinoLunticksApp.Models;
using KinoLunticksApp.Tools;
using Microsoft.EntityFrameworkCore;

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
        Button _previouButton;

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

            int showId = int.Parse(button.Tag.ToString());

            Showing selectedShowing = GetShowingById(showId);

            var sessionDetails = new SessionDetails 
            { 
                authorizedUser = _user,
                selectedMovie = _movie,
                navigationFrame = _frame,
                selectedShowing = selectedShowing
            };

            _frame.Navigate(new MoviePage(sessionDetails));
        }

        #region Methods
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

            if(_previouButton != null)
            {
                ResetButtonStyle(_previouButton);
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

            _previouButton = selectedButton;
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

        private Showing GetShowingById(int showId)
        {
            return _db.Showings.FirstOrDefault(s => s.ShowingId == showId);
        }
        #endregion
    }
}