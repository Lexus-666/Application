namespace kursah_5semestr.Abstractions
{
    public interface IUsersService
    {
        public Task<User> CreateUser(User user);

        public User? GetUserByLogin(string Token);
    }
}
