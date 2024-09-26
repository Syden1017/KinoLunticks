using System;
using System.Collections.Generic;

namespace KinoLunticksApp.Models;

public partial class Movie
{
    public string MovieCode { get; set; } = null!;

    public string MovieName { get; set; } = null!;

    public string MovieDescription { get; set; } = null!;

    public string MovieGenre { get; set; } = null!;

    public double MovieRating { get; set; }

    public string MovieDuration { get; set; } = null!;

    public string? AgeRestriction { get; set; }

    public decimal TicketPrice { get; set; }

    public byte[]? Preview { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
