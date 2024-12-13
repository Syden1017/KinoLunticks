using System;
using System.Collections.Generic;

namespace KinoLunticksApp.Models;

public partial class Hall
{
    public int HallId { get; set; }

    public string HallNumber { get; set; } = null!;

    public int Capacity { get; set; }

    public virtual ICollection<Row> Rows { get; set; } = new List<Row>();

    public virtual ICollection<Showing> Showings { get; set; } = new List<Showing>();
}
