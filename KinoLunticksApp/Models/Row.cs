using System;
using System.Collections.Generic;

namespace KinoLunticksApp.Models;

public partial class Row
{
    public int RowId { get; set; }

    public string RowNumber { get; set; } = null!;

    public int HallId { get; set; }

    public int SeatsInRow { get; set; }

    public virtual Hall Hall { get; set; } = null!;

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();

    public virtual ICollection<SelectedSeat> SelectedSeats { get; set; } = new List<SelectedSeat>();

    public virtual ICollection<TakenSeat> TakenSeats { get; set; } = new List<TakenSeat>();
}
