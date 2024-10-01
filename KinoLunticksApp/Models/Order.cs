using System;
using System.Collections.Generic;

namespace KinoLunticksApp.Models;

public partial class Order
{
    public string OrderNumber { get; set; } = null!;

    public string User { get; set; } = null!;

    public string Movie { get; set; } = null!;

    public TimeOnly SessionTime { get; set; }

    public string Seats { get; set; } = null!;

    public decimal Amount { get; set; }

    public string? Preview { get; set; }

    public virtual Movie MovieNavigation { get; set; } = null!;

    public virtual Movie? PreviewNavigation { get; set; }

    public virtual User UserNavigation { get; set; } = null!;
}
