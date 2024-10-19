using System.Drawing;
using System.IO;

using QRCoder;

namespace KinoLunticksApp.Tools
{
    public class QrGenerator
    {
        public string GenerateQRCodeFile(string text)
        {
            string filePath = Path.Combine(Path.GetTempPath(), "QRCode.png");

            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q))
                {
                    using (QRCode qrCode = new QRCode(qrCodeData))
                    {
                        using (Bitmap qrCodeBitmap = qrCode.GetGraphic(20))
                        {
                            qrCodeBitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                            return filePath;
                        }
                    }
                }
            }
        }
    }
}