using System;
using System.Collections.Generic;

namespace KinoLunticksApp.Models;

public partial class Seat
{
    public int SeatId { get; set; }

    public int RowId { get; set; }

    public string SeatNumber { get; set; } = null!;

    public virtual Row Row { get; set; } = null!;

    public virtual ICollection<SelectedSeat> SelectedSeats { get; set; } = new List<SelectedSeat>();

    public virtual ICollection<TakenSeat> TakenSeats { get; set; } = new List<TakenSeat>();
}
