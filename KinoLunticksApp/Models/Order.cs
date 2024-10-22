using System;
using System.Collections.Generic;

namespace KinoLunticksApp.Models;

public partial class Order
{
    public string User { get; set; } = null!;

    public int Movie { get; set; }

    public TimeOnly SessionTime { get; set; }

    public string Seats { get; set; } = null!;

    public decimal Amount { get; set; }

    public int OrderNumber { get; set; }

    public virtual Movie MovieNavigation { get; set; } = null!;

    public virtual User UserNavigation { get; set; } = null!;
}
