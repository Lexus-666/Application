using System;
using System.Collections.Generic;

namespace kursah_5semestr;

public partial class Transaction
{
    public Guid Id { get; set; }

    public string TransactionDetails { get; set; } = null!;

    public Guid UserId { get; set; }

    public User User { get; set; }

    public double TotalPrice { get; set; }
}
