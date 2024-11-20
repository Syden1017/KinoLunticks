using System;
using System.Collections.Generic;

namespace KinoLunticksApp.Models;

public partial class Order
{
    public int OrderNumber { get; set; }

    public int UserId { get; set; }

    public int Showing { get; set; }

    public decimal Amount { get; set; }

    public int SelectedRow { get; set; }

    public virtual ICollection<SelectedSeat> SelectedSeats { get; set; } = new List<SelectedSeat>();

    public virtual Showing ShowingNavigation { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
