using System.Windows;
using System.Windows.Controls;

using KinoLunticksApp.Models;
using KinoLunticksApp.Windows;

using Microsoft.EntityFrameworkCore;

namespace KinoLunticksApp.Pages
{
    /// <summary>
    /// Interaction logic for AdminPanelPage.xaml
    /// </summary>
    public partial class AdminPanelPage : Page
    {
        KinoLunticsContext _db = new KinoLunticsContext();

        Frame _frame;

        public AdminPanelPage(Frame frame)
        {
            InitializeComponent();

            _frame = frame;
            _db.Movies.Load();

            tableView.ItemsSource = _db.Movies.ToList();
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            var addEditMovieWindow = new AddEditMovieWindow(null);
            addEditMovieWindow.ShowDialog();
        }

        private void changeButton_Click(object sender, RoutedEventArgs e)
        {
            var movieForUpdate = tableView.SelectedItem;

            if (movieForUpdate == null)
            {
                MessageBox.Show(
                    "Выберите фильм для изменения",
                    "Предупреждение",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                    );
            }

            //var addEditMovieWindow = new AddEditMovieWindow(movieForUpdate);

            tableView.ItemsSource = _db.Movies.ToList();
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            var moviesForRemoving = tableView.SelectedItems.Cast<Movie>().ToList();

            if (moviesForRemoving.Count == 0)
            {
                MessageBox.Show(
                    "Выберите, пожалуйста, фильм, который хотите удалить",
                    "Внимание",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            if (MessageBox.Show(
                    $"Вы точно хотите удалить записи в количестве {moviesForRemoving.Count()} элементов?",
                    "Внимание",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    _db.Movies.RemoveRange(moviesForRemoving);

                    MessageBox.Show(
                        "Запись удалена",
                        "Информация",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                        );

                    _db.SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }

            tableView.ItemsSource = _db.Movies.ToList();
        }
    }
}
