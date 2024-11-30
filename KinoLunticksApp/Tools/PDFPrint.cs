using System.IO;
using System.Diagnostics;

using iText.Layout;
using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Properties;

using KinoLunticksApp.Models;
using iText.Kernel.Exceptions;
using System.Windows.Forms;

namespace KinoLunticksApp.Tools
{
    public class PDFPrint
    {
        QrGenerator generator = new QrGenerator();

        public void PrintToPDF(Order ticket)
        {
            // Создаем новый PDF документ
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Билет.pdf");

            try
            {
                using (PdfWriter writer = new PdfWriter(filePath))
                {
                    using (PdfDocument pdf = new PdfDocument(writer))
                    {
                        Document document = new Document(pdf);

                        // Заголовок
                        var titleParagraph = new Paragraph(ticket.ShowingNavigation.Movie.MovieName)
                            .SetFontSize(16)
                            .SetTextAlignment(TextAlignment.CENTER);
                        document.Add(titleParagraph);

                        // Добавляем дату и время показа
                        document.Add(new Paragraph($"Дата: {ticket.ShowingNavigation.ShowingDate.ToShortDateString()}").SetFontSize(12));
                        document.Add(new Paragraph($"Время: {ticket.ShowingNavigation.ShowingTime.ToShortTimeString()}").SetFontSize(12));

                        // Добавляем зал и места
                        document.Add(new Paragraph($"Зал: {ticket.ShowingNavigation.Hall.HallNumber}").SetFontSize(12));
                        document.Add(new Paragraph($"Места: {ticket.SelectedSeats}").SetFontSize(12));

                        // Генерируем QR-код и добавляем его в PDF
                        //string qrCodeFilePath = generator.GenerateQRCodeFile(ticket.ShowingNavigation.Movie.MovieName);
                        //Image qrImage = new Image(ImageDataFactory.Create(qrCodeFilePath));
                        //qrImage.SetWidth(100); // Устанавливаем ширину QR-кода
                        //qrImage.SetHeight(100); // Устанавлива6ем высоту QR-кода
                        //qrImage.SetTextAlignment(TextAlignment.CENTER);
                        //document.Add(qrImage);

                        // Закрываем документ
                        document.Close();
                    }
                }
            }
            catch (PdfException pdfEx)
            {
                MessageBox.Show($"Ошибка PDF: {pdfEx.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Общая ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Открываем PDF файл
            Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
        }
    }
}