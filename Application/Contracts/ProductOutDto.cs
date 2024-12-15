namespace kursah_5semestr.Contracts
{
    public record ProductOutDto(
        Guid Id,
        string? Title,
        double? Price,
        int? Quantity,
        string? Description
        );

}
