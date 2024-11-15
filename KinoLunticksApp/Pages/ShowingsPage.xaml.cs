using System.Windows;
using System.Windows.Controls;

using KinoLunticksApp.Tools;
using KinoLunticksApp.Models;

using Microsoft.EntityFrameworkCore;

namespace KinoLunticksApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для ShowingsPage.xaml
    /// </summary>
    public partial class ShowingsPage : Page
    {
        KinoLunticsContext _db = new KinoLunticsContext();
        ShowingImportService import = new ShowingImportService();

        Frame _frame;

        public ShowingsPage(Frame frame)
        {
            InitializeComponent();

            _frame = frame;

            _db.Showings.Include(s => s.Movie).Include(s => s.Hall).Load();

            tableView.ItemsSource = _db.Showings.ToList();
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnImportShowings_Click(object sender, RoutedEventArgs e)
        {
            import.ImportShowingsFromExcelAsync();

            tableView.ItemsSource = _db.Showings.ToList();
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

            tableView.ItemsSource = _db.Showings.ToList();
        }
    }
}
