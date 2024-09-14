using System.Windows.Controls;

namespace KinoLunticksApp.Pages
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        Frame _frame;

        public MainPage(Frame frame)
        {
            InitializeComponent();

            _frame = frame;
        }
    }
}
