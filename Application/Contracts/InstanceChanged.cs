namespace kursah_5semestr.Contracts
{
    public record InstanceChanged(
        string Action,
        string Entity,
        Dictionary<String, object> Data
        );
}
