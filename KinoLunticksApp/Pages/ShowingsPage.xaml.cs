using System.Windows;
using System.Windows.Controls;

using KinoLunticksApp.Tools;
using KinoLunticksApp.Models;

using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Windows.Data;

namespace KinoLunticksApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для ShowingsPage.xaml
    /// </summary>
    public partial class ShowingsPage : Page
    {
        KinoLunticsContext _db = new KinoLunticsContext();
        List<Showing> _showings = new List<Showing>();
        ShowingImportService import = new ShowingImportService();

        Frame _frame;

        public ShowingsPage(Frame frame)
        {
            InitializeComponent();

            _frame = frame;

            UpdateShowingsList();
        }

        private void UpdateShowingsList()
        {
            _db.Showings.Include(s => s.Movie).Include(s => s.Hall).Load();
            _showings = _db.Showings.Local.ToList();

            var groupedShowings = _showings
                .GroupBy(s => s.Movie.MovieName)
                .Select(g => new MovieGroup
                {
                    Title = g.Key,
                    Showings = g.ToList()
                }).ToList();

            tableView.ItemsSource = groupedShowings;
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnImportShowings_Click(object sender, RoutedEventArgs e)
        {
            import.ImportShowingsFromExcelAsync(tableView);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            _frame.GoBack();
        }

        private void changeButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            var showingForRemove = (sender as Button).DataContext as Showing;

            MessageBoxResult result = MessageBox.Show(
                                          "Вы точно хотите удалить запись?",
                                          "Внимание",
                                          MessageBoxButton.YesNo,
                                          MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _db.Showings.Remove(showingForRemove);

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

            UpdateShowingsList();
        }
    }
}