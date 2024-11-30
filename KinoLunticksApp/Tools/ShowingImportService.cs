using System.IO;
using System.Data;
using System.Text;
using System.Globalization;
using System.Windows.Forms;

using ExcelDataReader;

using KinoLunticksApp.Models;

using Microsoft.EntityFrameworkCore;

namespace KinoLunticksApp.Tools
{
    public class ShowingImportService
    {
        KinoLunticsContext _db = new KinoLunticsContext();
        List<Showing> _showings = new List<Showing>();

        public async Task ImportShowingsFromExcelAsync(System.Windows.Controls.TreeView treeView)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Excel Files | *.xls;*.xlsx;*.xlsm";
                openFileDialog.Title = "Выберите файл Excel с данными фильмов";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var filePath = openFileDialog.FileName;

                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                    try
                    {
                        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                        {
                            using (var reader = ExcelReaderFactory.CreateReader(stream))
                            {
                                var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                                {
                                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                                    {
                                        UseHeaderRow = true
                                    }
                                });
                                var dataTable = result.Tables[0];

                                foreach (DataRow row in dataTable.Rows)
                                {
                                    if (reader.Depth == 0) continue;

                                    var movieName = row["MovieName"].ToString();
                                    var movieId = _db.Movies.Where(m => m.MovieName == movieName).
                                                             Select(m => m.MovieCode).FirstOrDefault();

                                    var hallNumber = row["HallNumber"].ToString();
                                    var hallId = _db.Halls.Where(h => h.HallNumber == hallNumber).
                                                           Select(h => h.HallId).FirstOrDefault();

                                    if (movieId != null && hallId != null)
                                    {
                                        try
                                        {
                                            string dateString = row["Date"].ToString();
                                            string dateOnlyString = dateString.Split(' ')[0];

                                            DateOnly date = DateOnly.ParseExact(dateOnlyString, "dd.MM.yyyy", CultureInfo.InvariantCulture);

                                            string timeString = row["Time"].ToString();
                                            string timeOnlyString = timeString.Split(' ')[1];

                                            TimeOnly time = TimeOnly.ParseExact(timeOnlyString, "HH:mm:ss", CultureInfo.InvariantCulture);

                                            Showing showing = new Showing
                                            {
                                                MovieId = Convert.ToInt32(movieId),
                                                HallId = Convert.ToInt32(hallId),
                                                ShowingDate = date,
                                                ShowingTime = time
                                            };
                                            _db.Showings.Add(showing);


                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show(
                                                $"Ошибка формата: {ex.Message}",
                                                "Ошибка",
                                                MessageBoxButtons.OK,
                                                MessageBoxIcon.Error
                                                );
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show(
                                            $"Фильм '{movieName}' или зал '{hallNumber}' не найдены.",
                                            "Ошибка",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Warning
                                            );
                                    }
                                }

                                await _db.SaveChangesAsync();
                                MessageBox.Show(
                                    "Импорт произведен успешно",
                                    "Выполнение импорта",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Ошибка при импорте данных: {ex.Message}",
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }

                    _db.Showings.Include(s => s.Movie).Include(s => s.Hall).Load();
                    _showings = _db.Showings.Local.ToList();

                    var groupedShowings = _showings
                        .GroupBy(s => s.Movie.MovieName)
                        .Select(g => new MovieGroup
                        {
                            Title = g.Key,
                            Showings = g.ToList()
                        }).ToList();

                    treeView.ItemsSource = groupedShowings;
                }
            }
        }
    }
}