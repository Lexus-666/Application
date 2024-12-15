using kursah_5semestr;

namespace kursah_5semestr;

public partial class OrderDetails
{
    public OrderDetails() { }

    public OrderDetails(Guid orderId, Product product, int quantity, double price)
    {
        Id = Guid.NewGuid();
        OrderId = orderId;
        ProductId = product.Id;
        Product = product;
        Quantity = quantity;
        Price = price;
    }
}