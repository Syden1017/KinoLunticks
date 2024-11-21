using System;
using System.Collections.Generic;

namespace KinoLunticksApp.Models;

public partial class TakenSeat
{
    public int ShowingId { get; set; }

    public int SeatId { get; set; }

    public int RowNumber { get; set; }

    public virtual Row RowNumberNavigation { get; set; } = null!;

    public virtual Seat Seat { get; set; } = null!;

    public virtual Showing Showing { get; set; } = null!;
}
