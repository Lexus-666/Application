using kursah_5semestr.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace kursah_5semestr.Services
{
    public class CartItemsService : ICartItemsService
    {
        private AppDbContext _context;

        public CartItemsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CartItem> CreateCartItem(CartItem CartItem)
        {
            CartItem.Id = Guid.NewGuid();
            _context.CartItems.Add(CartItem);
            await _context.SaveChangesAsync();
            return CartItem;
        }

        public async Task<CartItem?> UpdateCartItem(Guid id, CartItem patch)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var CartItem = GetCartItemById(id);
                    if (CartItem != null)
                    {
                        if (patch.Quantity != null)
                        {
                            CartItem.Quantity = (int)patch.Quantity;
                        }
                        _context.CartItems.Update(CartItem);
                        await _context.SaveChangesAsync();
                        transaction.Commit();
                        return CartItem;
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return null;
                }
            }
            return null;
        }

        public IList<CartItem> GetCartItems()
        {
            return _context.CartItems.Include(ci => ci.Product).ToList();
        }

        public CartItem? GetCartItemById(Guid id)
        {
            return _context.CartItems.Find(id);
        }

        public async Task<bool> DeleteCartItem(Guid id)
        {
            var CartItem = await _context.CartItems.FindAsync(id);
            if (CartItem != null)
            {
                _context.CartItems.Remove(CartItem);
                await _context.SaveChangesAsync();
                return true;
            }
            else { return false; }
        }
    }
}