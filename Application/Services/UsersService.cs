﻿using kursah_5semestr.Abstractions;
using kursah_5semestr.Contracts;
using Microsoft.EntityFrameworkCore;

namespace kursah_5semestr.Services
{
    public class UsersService : IUsersService
    {
        private AppDbContext _context;

        public UsersService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> CreateUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public User? GetUserByLogin(string login)
        {
            return _context.Users
                .Include(u => u.CartItems)
                .ThenInclude(ci => ci.Product)
                .Where(u => u.Login == login)
                .First();
        }

        public async Task RemoveCartItems(User user)
        { 
            user.CartItems = new List<CartItem>();
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
