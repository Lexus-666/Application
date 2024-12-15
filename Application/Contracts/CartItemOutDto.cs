namespace kursah_5semestr.Contracts
{
    public record CartItemOutDto(
        Guid Id,
        Product? Product,
        int? Quantity
        );
}
