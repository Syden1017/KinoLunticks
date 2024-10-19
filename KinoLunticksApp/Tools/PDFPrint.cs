using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

using KinoLunticksApp.Models;

using PdfSharp.Pdf;
using PdfSharp.Drawing;

namespace KinoLunticksApp.Tools
{
    public class PDFPrint
    {
        QrGenerator generator = new QrGenerator();

        public void PrintToPDF(Order ticket)
        {
            // Создаем новый PDF документ и задаем заголовок
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Билет";

            // Создаем страницу
            PdfPage page = document.AddPage();
            page.Size = PdfSharp.PageSize.A4;

            XGraphics gfx = XGraphics.FromPdfPage(page);

            //// Создаем шрифт
            XFont font = new XFont("Verdana", 20, XFontStyleEx.Bold);
            XFont fontRegular = new XFont("Verdana", 20, XFontStyleEx.Regular);

            // Печатаем заголовок
            gfx.DrawString(ticket.MovieNavigation.MovieName, font, XBrushes.Black, new XRect(0, 0, page.Width, page.Height), XStringFormats.TopLeft);

            int yPoint = 30;
            gfx.DrawString(ticket.Seats, fontRegular, XBrushes.Black, new XRect(0, yPoint, page.Width, page.Height), XStringFormats.TopLeft);

            yPoint += 30;
            gfx.DrawString("Время сеанса: ", font, XBrushes.Black, new XRect(0, yPoint, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString(ticket.SessionTime.ToString(), fontRegular, XBrushes.Black, new XRect(170, yPoint, page.Width, page.Height), XStringFormats.TopLeft);

            yPoint += 30;
            gfx.DrawString("Оплачено:", font, XBrushes.Black, new XRect(0, yPoint, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString(ticket.Amount.ToString(), fontRegular, XBrushes.Black, new XRect(130, yPoint, page.Width, page.Height), XStringFormats.TopLeft);

            // Генерируем QR-код и получаем путь к файлу
            string qrCodeFilePath = generator.GenerateQRCodeFile(ticket.MovieNavigation.MovieName);

            XImage xImage = XImage.FromFile(qrCodeFilePath);

            // Определяем позицию для QR-кода
            double qrCodeX = (page.Width - xImage.PixelWidth * 72 / xImage.HorizontalResolution) / 2; // Центрируем по горизонтали
            double qrCodeY = yPoint + 30;                                                             // Позиция ниже последнего текста с отступом
            gfx.DrawImage(xImage, qrCodeX, qrCodeY, 100, 100);                                        // Рисуем QR-код на странице

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*", // Фильтр для типов файлов
                Title = "Сохранить PDF файл",
                FileName = "Билет.pdf"                                  // Имя файла по умолчанию
            };

            // Показываем диалоговое окно и проверяем, нажал ли пользователь "Сохранить"
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName; // Получаем полный путь к файлу
                document.Save(filePath);
                MessageBox.Show(
                    $"Документ успешно сохранен по пути: {filePath}", 
                    "Сохранение", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Information
                    );

                File.Delete(qrCodeFilePath);

                try
                {
                    Process.Start(new ProcessStartInfo(filePath) 
                    { 
                        UseShellExecute = true 
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Не удалось открыть файл: {ex.Message}");
                }
            }
        }
    }
}