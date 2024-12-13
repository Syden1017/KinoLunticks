using System;
using System.Collections.Generic;

namespace KinoLunticksApp.Models;

public partial class Movie
{
    public int MovieCode { get; set; }

    public string MovieName { get; set; } = null!;

    public string MovieDescription { get; set; } = null!;

    public double MovieRating { get; set; }

    public string MovieDuration { get; set; } = null!;

    public string ProducerName { get; set; } = null!;

    public string AgeRestriction { get; set; } = null!;

    public decimal TicketPrice { get; set; }

    public string? Preview { get; set; }

    public virtual ICollection<Showing> Showings { get; set; } = new List<Showing>();

    public virtual ICollection<Actor> Actors { get; set; } = new List<Actor>();

    public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();
}
