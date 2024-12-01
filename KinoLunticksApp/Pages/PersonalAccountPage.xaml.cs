using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

using KinoLunticksApp.Tools;
using KinoLunticksApp.Models;
using KinoLunticksApp.Windows;

using Microsoft.EntityFrameworkCore;
using System.Windows.Media;
using System.Collections.ObjectModel;

namespace KinoLunticksApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для PersonalAccountPage.xaml
    /// </summary>
    public partial class PersonalAccountPage : Page
    {
        KinoLunticsContext _db = new KinoLunticsContext();
        User _user = new User();
        ImageWork _image = new ImageWork();
        PDFPrint _print = new PDFPrint();

        public List<Ticket> Tickets { get; set; }

        Frame _frame;

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

        public PersonalAccountPage(Frame frame, User authorizedUser)
        {
            InitializeComponent();

            _frame = frame;
            _user = authorizedUser;

            _currentImage = _defaultImage;

            _imageData = _user.Photo;

            if (_user.Photo != null)
            {
                _image.ReadBitmapImageFromArray(new MemoryStream(_user.Photo), out _currentImage);
            }

            DataContext = _user;

            LoadTickets();
            UpdateCardsList();
        }

        private void LoadTickets()
        {
            _db.Orders.Include("ShowingNavigation").Include(o => o.ShowingNavigation.Movie).
                    Include(o => o.ShowingNavigation.Hall).
                    Where(o => o.User.UserId == _user.UserId).
                Load();

            lViewMyTickets.ItemsSource = _db.Orders.Local.ToList();
        }

        private void UpdateCardsList()
        {
            _db.Users.Include(u => u.AccountNumbers).FirstOrDefault(u => u.Login == _user.Login);

            lViewMyCards.ItemsSource = _db.BankAccounts.Local.ToList();
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
            _db.Entry(_user).State = EntityState.Modified;

            try
            {
                _user.Photo = _imageData;

                _db.SaveChanges();

                MessageBox.Show(
                    "Информация сохранена",
                    "Сохранение",
                     MessageBoxButton.OK,
                     MessageBoxImage.Information
                    );
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
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            var ticket = (sender as Button).DataContext as Order;

            _print.PrintToPDF(ticket);
        }

        private void btnAddCard_Click(object sender, RoutedEventArgs e)
        {
            AddCardWindow addCard = new AddCardWindow(_user);
            addCard.ShowDialog();

            UpdateCardsList();
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