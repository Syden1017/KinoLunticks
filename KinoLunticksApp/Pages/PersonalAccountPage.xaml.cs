using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

using KinoLunticksApp.Models;
using KinoLunticksApp.Tools;

using Microsoft.EntityFrameworkCore;

namespace KinoLunticksApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для PersonalAccountPage.xaml
    /// </summary>
    public partial class PersonalAccountPage : Page
    {
        KinoLunticsContext _db = new KinoLunticsContext();
        User _user = new User();

        Frame _frame;

        ImageWork _image = new ImageWork();

        static string _workingDirectory = Directory.GetParent(
                                    Directory.GetParent(
                                        Directory.GetParent(
                                            Environment.CurrentDirectory).
                                                            FullName).
                                                        FullName).
                                                       FullName;

        BitmapImage _currentImage,
                    _defaultImage = new BitmapImage(new Uri(_workingDirectory +
                                                            @"\Images\default.JPG"));

        byte[] _imageData;

        public PersonalAccountPage(Frame frame, User user)
        {
            InitializeComponent();

            _frame = frame;
            _user = user;
            _currentImage = _defaultImage;

            if (_user.Photo != null)
            {
                _image.ReadBitmapImageFromArray(new MemoryStream(_user.Photo), out _currentImage);
            }

            DataContext = _user;

            UpdateOrdersList();
        }

        private void UpdateOrdersList()
        {
            _db.Orders.Include("MovieNavigation").Load();

            lViewMyTickets.ItemsSource = _db.Orders.Local.ToList();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (dPicBirthDate.SelectedDate == DateTime.MinValue)
            {
                dPicBirthDate.SelectedDate = new DateTime(2000, 1, 1);
            }
        }

        private void btnGoBack_Click(object sender, RoutedEventArgs e)
        {
            _frame.GoBack();
        }

        private void imgPhoto_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BitmapImage photo = GetImage();

            if (photo != null)
            {
                imgPhoto.Source = photo;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Получение изображения
        /// </summary>
        /// <returns>Загруженное изображение</returns>
        private BitmapImage GetImage()
        {
            _image.OpenImage(ref _imageData, ref _currentImage);

            return _currentImage;
        }
    }
}