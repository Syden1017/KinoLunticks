using System;
using System.Collections.Generic;

namespace KinoLunticksApp.Models;

public partial class Showing
{
    public int ShowingId { get; set; }

    public int MovieId { get; set; }

    public int HallId { get; set; }

    public DateOnly ShowingDate { get; set; }

    public TimeOnly ShowingTime { get; set; }

    public virtual Hall Hall { get; set; } = null!;

    public virtual Movie Movie { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
}
