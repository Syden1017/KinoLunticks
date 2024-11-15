using System;
using System.Collections.Generic;

namespace KinoLunticksApp.Models;

public partial class SelectedSeat
{
    public int OrderId { get; set; }

    public int SeatId { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Seat Seat { get; set; } = null!;
}
