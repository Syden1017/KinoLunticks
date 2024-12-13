using System;
using System.Collections.Generic;

namespace KinoLunticksApp.Models;

public partial class BankAccount
{
    public string AccountNumber { get; set; } = null!;

    public string ExpirationDate { get; set; } = null!;

    public string Cvccode { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
