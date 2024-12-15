using System;
using System.Collections.Generic;

namespace kursah_5semestr;

public partial class Product
{
    public Guid Id { get; set; }

    public string? Title { get; set; } = null!;

    public double? Price { get; set; }

    public int? Quantity { get; set; }

    public string? Description { get; set; } = null!;
}
