using System.Security.Cryptography.X509Certificates;

namespace kursah_5semestr.Contracts
{
    public record StatusOutDto(
        string Status,
        string? Message = null
        );
}
