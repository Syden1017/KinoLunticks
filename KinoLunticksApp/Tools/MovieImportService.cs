using System.IO;
using System.Text;
using System.Data;
using System.Windows.Forms;

using ExcelDataReader;

using KinoLunticksApp.Models;

using Microsoft.EntityFrameworkCore;

namespace KinoLunticksApp.Tools
{
    public class MovieImportService
    {
        private readonly KinoLunticsContext _db = new KinoLunticsContext();

        public async Task ImportMoviesFromExcelAsync(System.Windows.Controls.ListView listView)
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

                                    var movieName = row["Title"].ToString();
                                    var movie = await _db.Movies
                                                         .Include(m => m.Actors)
                                                         .Include(m => m.Genres)
                                                         .FirstOrDefaultAsync(m => m.MovieName == movieName);

                                    var coverImageValue = row["CoverImage"].ToString();

                                    if (movie == null)
                                    {
                                        movie = new Movie
                                        {
                                            MovieName = movieName,
                                            MovieDescription = row["Description"].ToString(),
                                            MovieRating = Convert.ToDouble(row["Rating"]),
                                            MovieDuration = row["Duration"].ToString(),
                                            ProducerName = row["Director"].ToString(),
                                            AgeRestriction = row["AgeRating"].ToString(),
                                            TicketPrice = Convert.ToDecimal(row["TicketPrice"]),
                                            Preview = coverImageValue == "NULL" ? null : coverImageValue
                                        };
                                        _db.Movies.Add(movie);
                                    }

                                    //await AddActorsToMovieAsync(row, movie);
                                    //await AddGenresToMovieAsync(row, movie);
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

                    listView.ItemsSource = _db.Movies.ToList();
                }
            }
        }

        //private async Task AddActorsToMovieAsync(DataRow row, Movie movie)
        //{
        //    var actorEntries = row["Actors"].ToString().Split(',');

        //    try
        //    {
        //        foreach (var actorEntry in actorEntries)
        //        {
        //            var actorDetails = actorEntry.Split(' ');
        //            if (actorDetails.Length < 2) continue;

        //            var firstName = actorDetails[0].Trim();
        //            var lastName = actorDetails[1].Trim();

        //            var actor = await _db.Actors.FirstOrDefaultAsync(a => a.ActorName == firstName &&
        //                                                                  a.ActorLastName == lastName);

        //            if (actor == null)
        //            {
        //                actor = new Actor
        //                {
        //                    ActorName = firstName,
        //                    ActorLastName = lastName
        //                };
        //                _db.Actors.Add(actor);
        //                await _db.SaveChangesAsync();
        //            }

        //            if (!movie.Actors.Any(a => a.ActorName == actor.ActorName &&
        //                                       a.ActorLastName == actor.ActorLastName))
        //            {
        //                movie.Actors.Add(actor);
        //            }
        //        }

        //        await _db.SaveChangesAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(
        //            $"Ошибка при импорте данных: {ex.Message}",
        //            "Ошибка",
        //            MessageBoxButtons.OK,
        //            MessageBoxIcon.Error);
        //    }
        //}

        //private async Task AddGenresToMovieAsync(DataRow row, Movie movie)
        //{
        //    var genreNames = row["Genres"].ToString().Split(',');

        //    try
        //    {
        //        foreach (var genreName in genreNames)
        //        {
        //            var trimmedGenreName = genreName.Trim();

        //            var genre = await _db.Genres.FirstOrDefaultAsync(g => g.GenreName == trimmedGenreName);

        //            if (genre == null)
        //            {
        //                genre = new Genre
        //                {
        //                    GenreName = trimmedGenreName
        //                };

        //                _db.Genres.Add(genre);
        //                await _db.SaveChangesAsync();
        //            }

        //            if (!movie.Genres.Any(g => g.GenreName == genre.GenreName))
        //            {
        //                movie.Genres.Add(genre);
        //            }
        //        }

        //        await _db.SaveChangesAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(
        //            $"Ошибка при импорте данных: {ex.Message}",
        //            "Ошибка",
        //            MessageBoxButtons.OK,
        //            MessageBoxIcon.Error);
        //    }
        //}
    }
}