using System.Windows.Controls;

using KinoLunticksApp.Models;

namespace KinoLunticksApp.Tools
{
    public class OrderDetails
    {
        public User? authorizedUser { get; set; }
        public Movie? selectedMovie { get; set; }
        public Frame? navigationFrame { get; set; }
        public Showing? selectedShowing { get; set; }
        public string? selectedPlaces { get; set; }
        public decimal fullAmount { get; set; }
    }
}