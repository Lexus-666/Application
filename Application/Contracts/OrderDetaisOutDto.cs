using System.Security.Cryptography.X509Certificates;

namespace kursah_5semestr.Contracts
{
    public record OrderDetailsOutDto(
        Guid ProductId,
        int Quantity,
        double Price
        );
}
