using kursah_5semestr.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace kursah_5semestr.Services
{
    public class CartItemsService : ICartItemsService
    {
        private AppDbContext _context;
        private ILogger _logger;

        public CartItemsService(AppDbContext context, ILogger<CartItemsService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<CartItem> CreateCartItem(CartItem CartItem)
        {
            CartItem.Id = Guid.NewGuid();
            _context.CartItems.Add(CartItem);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Created cart item {CartItem.Id}");
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
                        _logger.LogInformation($"Updated cart item {id}");
                        return CartItem;
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.LogError(ex, $"Error updating cart item {id}");
                    return null;
                }
            }
            return null;
        }

        public IList<CartItem> GetCartItems(Guid userId)
        {
            return _context.CartItems.Where(ci => ci.UserId == userId).Include(ci => ci.Product).ToList();
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
                _logger.LogInformation($"Deleted cart item {id}");
                return true;
            }
            else {
                _logger.LogWarning($"Cart item {id} not found");
                return false; 
            }
        }
    }
}