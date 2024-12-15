namespace kursah_5semestr;

public partial class Product
{
    public const int MinQuantity = 0;
    public const double MinPrice = 0.01;
    public const int MaxDescriptionLength = 1024;
    public const int MaxTitleLength = 128;

    public Product() { }

    private Product(string title, double price, int quantity, string description)
    {
        Title = title;
        Price = price;
        Quantity = quantity;
        Description = description;
    }

    private static string BasicCheck(int? quantity, string? title, double? price, string? description)
    {
        var error = string.Empty;

        if (string.IsNullOrWhiteSpace(title) || title.Length > MaxTitleLength)
        {
            error = "Title cannot be empty or longer than 128.";
        }

        else if (quantity == null || quantity < MinQuantity)
        {
            error = $"Quantity must be greater than {MinQuantity - 1}";
        }

        else if (price == null || price < MinPrice)
        {
            error = $"Price must be greater than {MinPrice}";
        }

        else if (string.IsNullOrWhiteSpace(description) || description.Length > MaxDescriptionLength)
        {
            error = "Description cannot be empty or longer than 1024.";
        }

        return error;
    }

    public static (Product Product, string Error) Create(string? title, double? price, int? quantity, string? description)
    {
        var error = BasicCheck(quantity, title, price, description);

        var product = new Product(title!, (double)price!, (int)quantity!, description!);
        
        return (product, error);
    }
}