using System.Security.Cryptography.X509Certificates;

namespace kursah_5semestr.Contracts
{
    public record OrderOutDto(
        Guid Id,
        DateTime Date,
        string Status,
        double Amount,
        IList<OrderDetailsOutDto> OrderDetails,
        Guid? UserId = null
        );
}
