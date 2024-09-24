using Microsoft.Win32;

using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace KinoLunticksApp.Tools
{
    class ImageWork
    {
        /// <summary>
        /// Считывание изображения из массива байт
        /// </summary>
        /// <param name="byteStream">Поток на основе массива байт</param>
        /// <param name="image">Формируемое изображение</param>
        public void ReadBitmapImageFromArray(MemoryStream byteStream, out BitmapImage image)
        {
            image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = byteStream;
            image.EndInit();
        }

        /// <summary>
        /// Загрузка изображения
        /// </summary>
        /// <param name="imageData">Массив байт для записи изображения</param>
        /// <param name="image">Загруженное изображение</param>
        public void OpenImage(ref byte[] imageData, ref BitmapImage image)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Выбор фото";
            openFileDialog.Filter = "Image files (*.jpeg;*.jpg;*.png)|*.jpeg;*.jpg;*.png|" +
                                    "Jpg files (*.jpeg;*.jpg)|*.jpeg;*.jpg|" +
                                    "Png files (*.png)|*.png";
            openFileDialog.FilterIndex = 1;

            if (openFileDialog.ShowDialog().Value)
            {
                try
                {
                    using (FileStream fileStream = new FileStream(openFileDialog.FileName, FileMode.Open))
                    {
                        imageData = new byte[fileStream.Length];
                        fileStream.Read(imageData, 0, imageData.Length);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        ex.Message.ToString(),
                        "Системная ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                        );
                }

                ReadBitmapImageFromArray(new MemoryStream(imageData), out image);
            }
        }

        /// <summary>
        /// Получение изображения
        /// </summary>
        /// <param name="imageData">Массив байт для записи изображения</param>
        /// <param name="currentImage">Загруженное изображение</param>
        /// <returns>Загруженное изображение</returns>
        public BitmapImage GetImage(byte[] imageData, BitmapImage currentImage)
        {
            OpenImage(ref imageData, ref currentImage);

            return currentImage;
        }
    }
}