using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

using PdfSharp.Pdf;
using PdfSharp.Drawing;

using KinoLunticksApp.Models;
using PdfSharp.UniversalAccessibility.Drawing;

namespace KinoLunticksApp.Tools
{
    public class PDFPrint
    {
        QrGenerator _qrGenerator = new QrGenerator();

        /// <summary>
        /// Создает PDF документ на основе купленного билета
        /// </summary>
        /// <param name="ticket">Купленный билет</param>
        public void PrintToPDF(Order ticket)
        {
            string finalFilePath = string.Empty;

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
                saveFileDialog.Title = "Сохранить PDF файл";
                saveFileDialog.FileName = "Билет.pdf";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    finalFilePath = saveFileDialog.FileName;
                }
                else
                {
                    return;
                }
            }

            try
            {
                PdfDocument pdfDocument = new PdfDocument();
                pdfDocument.Info.Title = "Билет на фильм";

                PdfPage pdfPage = pdfDocument.AddPage();
                pdfPage.Width = XUnit.FromMillimeter(90);
                pdfPage.Height = XUnit.FromMillimeter(60);

                XGraphics gfx = XGraphics.FromPdfPage(pdfPage);
                XFont fontTitle = new XFont("Verdana", 20, XFontStyleEx.Bold);
                XFont fontRegular = new XFont("Verdana", 16, XFontStyleEx.Regular);
                XBrush brush = CreateBrushFromHex("#2B2B2B");

                var selectedSeats = ticket.SelectedSeats;
                selectedSeats = selectedSeats.Replace("\r\n", "").Replace("\n", "").Replace("\r", "");

                gfx.DrawRectangle(brush, new XRect(0, 0, pdfPage.Width, pdfPage.Height));
                gfx.DrawString(ticket.ShowingNavigation.Movie.MovieName, fontTitle, XBrushes.White, new XRect(7, 20, pdfPage.Width, pdfPage.Height), XStringFormats.TopLeft);
                gfx.DrawString($"Дата: {ticket.ShowingNavigation.formattedShowingDate}", fontRegular, XBrushes.White, new XRect(7, 60, pdfPage.Width, pdfPage.Height), XStringFormats.TopLeft);
                gfx.DrawString($"Время: {ticket.ShowingNavigation.ShowingTime.ToShortTimeString()}", fontRegular, XBrushes.White, new XRect(7, 80, pdfPage.Width, pdfPage.Height), XStringFormats.TopLeft);
                gfx.DrawString($"{ticket.ShowingNavigation.Hall.HallNumber}", fontRegular, XBrushes.White, new XRect(7, 100, pdfPage.Width, pdfPage.Height), XStringFormats.TopLeft);
                gfx.DrawString(selectedSeats, fontRegular, XBrushes.White, new XRect(7, 120, pdfPage.Width, pdfPage.Height), XStringFormats.TopLeft);

                Bitmap qrCodeBitmap = _qrGenerator.InitializeQR(ticket.ShowingNavigation.Movie.MovieName);


                using (var stream = new MemoryStream())
                {
                    qrCodeBitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    stream.Position = 0;
                    XImage image = XImage.FromStream(stream);

                    double qrCodeWidth = 50;
                    double qrCodeHeight = 50;

                    double xPosition = pdfPage.Width - qrCodeWidth - 7;
                    double yPosition = 7;

                    gfx.DrawImage(image, new XRect(xPosition, yPosition, qrCodeWidth, qrCodeHeight));
                }

                pdfDocument.Save(finalFilePath);

                Process.Start(new ProcessStartInfo(finalFilePath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка: {ex.Message}\nСтек вызовов: {ex.StackTrace}",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Создает новую кисть на основе HEX кода цвета
        /// </summary>
        /// <param name="hexColor">Строка HEX кода цвета</param>
        /// <returns>Новая кисть</returns>
        public static XBrush CreateBrushFromHex(string hexColor)
        {
            if (hexColor.StartsWith("#"))
            {
                hexColor = hexColor.Substring(1);
            }

            int r = Convert.ToInt32(hexColor.Substring(0, 2), 16);
            int g = Convert.ToInt32(hexColor.Substring(2, 2), 16);
            int b = Convert.ToInt32(hexColor.Substring(4, 2), 16);

            XColor color = XColor.FromArgb(r, g, b);
            return new XSolidBrush(color);
        }
    }
}