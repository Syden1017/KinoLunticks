using System;
using System.Collections.Generic;

namespace KinoLunticksApp.Models;

public partial class Seat
{
    public int SeatId { get; set; }

    public int RowId { get; set; }

    public string SeatNumber { get; set; } = null!;

    public bool IsAvailable { get; set; }

    public virtual Row Row { get; set; } = null!;
}
