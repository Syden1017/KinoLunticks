using System;
using System.Collections.Generic;

namespace KinoLunticksApp.Models;

public partial class User
{
    public string Login { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string UserLastName { get; set; } = null!;

    public string EmailAddress { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateOnly? BirthDate { get; set; }

    public int UserRole { get; set; }

    public byte[]? Photo { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Role UserRoleNavigation { get; set; } = null!;

    public virtual ICollection<BankAccount> AccountNumbers { get; set; } = new List<BankAccount>();
}
