using kursah_5semestr;

namespace kursah_5semestr;

public partial class Order
{
    public Order() { }

    private Order(Guid id, DateTime date, User user, string status, double amount, ICollection<OrderDetails> details)
    {
        Id = id;
        UserId = user.Id;
        User = user;
        Date = date;
        Status = status;
        Amount = amount;
        OrderDetails = details;
    }

    private static string BasicCheck(double amount, User user, string status, IList<CartItem> cartItems)
    {
        var error = string.Empty;

        if (amount <= 0)
        {
            error = $"Amount must be greater than zero";
        }
        else if (cartItems == null || cartItems.Count == 0)
        {
            error = "Cart must have at least one item.";
        }
        else if (user == null)
        {
            error = "User must be specified.";
        }
        else if (string.IsNullOrWhiteSpace(status))
        {
            error = "Status cannot be empty or null.";
        }

        return error;
    }

    public static (Order? Order, string Error) Create(Guid Id, User user, string status, double amount, IList<CartItem> cartItems, ICollection<OrderDetails> details)
    {
        var error = BasicCheck(amount, user, status, cartItems);

        if (!string.IsNullOrEmpty(error))
        {
            return (null, error);
        }

        var order = new Order(Id, DateTime.UtcNow, user, status, amount, details);

        return (order, error);
    }
}