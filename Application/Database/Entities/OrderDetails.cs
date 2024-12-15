using System;
using System.Collections.Generic;

namespace kursah_5semestr;

public partial class OrderDetails
{

    public Guid Id { get; set; }

    public Guid OrderId { get; set; }

    public Guid ProductId { get; set; }

    public Product Product { get; set; }

    public int Quantity { get; set; }

    public double Price { get; set; }
}