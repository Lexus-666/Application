namespace kursah_5semestr.Abstractions
{
    public interface IBrokerService
    {
        public Task SendMessage(string exchange, object message);

        public Task Subscribe(string exchange, Func<string, Task> handler);
    }
}
