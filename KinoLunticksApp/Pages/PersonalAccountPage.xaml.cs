using System.Windows.Input;
using System.Windows.Controls;

using KinoLunticksApp.Models;

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

        public PersonalAccountPage(Frame frame, User user)
        {
            InitializeComponent();

            _frame = frame;
            _user = user;

            DataContext = _user;
        }

        private void imgPhoto_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
