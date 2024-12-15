using System;
using System.Collections.Generic;

namespace kursah_5semestr;

public partial class User
{
    public Guid Id { get; set; }

    public string Login { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}
