using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace KinoLunticksApp.Pages
{
    /// <summary>
    /// Interaction logic for RegistrationPage.xaml
    /// </summary>
    public partial class RegistrationPage : Page
    {
        Frame _frame;

        public RegistrationPage(Frame frame)
        {
            InitializeComponent();

            _frame = frame;
        }

        private void btnSignIn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void txtBlockAuthorization_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _frame.Navigate(new AutorizationPage(_frame));
        }
    }
}