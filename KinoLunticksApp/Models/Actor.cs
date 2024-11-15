using System;
using System.Collections.Generic;

namespace KinoLunticksApp.Models;

public partial class Actor
{
    public int ActorId { get; set; }

    public string ActorName { get; set; } = null!;

    public string ActorLastName { get; set; } = null!;

    public string? Photo { get; set; }

    public virtual ICollection<Movie> MovieCodes { get; set; } = new List<Movie>();
}
