namespace kursah_5semestr.Abstractions
{
    public interface IUsersRepository
    {
        public Task RemoveCartItems(User user);
    }
}
