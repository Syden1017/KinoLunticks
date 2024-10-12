using System;
using System.Collections.Generic;

namespace KinoLunticksApp.Models;

public partial class Genre
{
    public int GenreId { get; set; }

    public string GenreName { get; set; } = null!;

    public virtual ICollection<Movie> MovieCodes { get; set; } = new List<Movie>();
}
