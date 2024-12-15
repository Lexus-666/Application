namespace kursah_5semestr;

public partial class CartItem
{
    public const int MinQuantity = 1;

    public CartItem() { }

    private CartItem(Product product, User user, int quantity)
    {
        Product = product;
        User = user;
        Quantity = quantity;
    }

    private static string BasicCheck(int quantity)
    {
        var error = string.Empty;

        if (quantity < MinQuantity)
        {
            error = $"Quantity must be greater than {MinQuantity - 1}";
        }

        return error;
    }

    public static (CartItem CartItem, string Error) Create(Product product, User user, int quantity)
    {
        var error = BasicCheck(quantity);

        var cartItem = new CartItem(product, user, quantity);

        return (cartItem, error);
    }
}