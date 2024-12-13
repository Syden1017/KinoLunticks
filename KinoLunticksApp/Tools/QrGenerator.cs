using System.Drawing;

using ZXing;
using ZXing.QrCode;
using ZXing.Windows.Compatibility;

namespace KinoLunticksApp.Tools
{
    public class QrGenerator
    {
        public Bitmap InitializeQR(string content)
        {
            QrCodeEncodingOptions options = new()
            {
                DisableECI = true,
                PureBarcode = true,
                CharacterSet = "UTF-8",
                Width = 450,
                Height = 450
            };

            BarcodeWriter bw = new()
            {
                Format = BarcodeFormat.QR_CODE,
                Options = options
            };
            
            return bw.Write(content);
        }
    }
}