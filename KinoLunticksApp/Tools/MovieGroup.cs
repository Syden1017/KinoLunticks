using KinoLunticksApp.Models;

namespace KinoLunticksApp.Tools
{
    public class MovieGroup
    {
        public string Title { get; set; }
        public List<Showing> Showings { get; set; }
    }
}
