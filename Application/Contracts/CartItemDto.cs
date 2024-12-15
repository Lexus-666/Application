namespace kursah_5semestr.Contracts
{
    public record CartItemDto(
        Guid? ProductId,
        Guid? UserId,
        int Quantity
        );
}
