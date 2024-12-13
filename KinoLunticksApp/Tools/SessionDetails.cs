using System.Windows.Controls;

using KinoLunticksApp.Models;

namespace KinoLunticksApp.Tools
{
    public class SessionDetails
    {
        public User? authorizedUser { get; set; }
        public Movie? selectedMovie { get; set; }
        public Frame? navigationFrame { get; set; }
        public Showing? selectedShowing { get; set; }
    }
}
