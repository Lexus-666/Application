namespace kursah_5semestr.Abstractions
{
    public interface ICartItemsService
    {
        public Task<CartItem> CreateCartItem(CartItem CartItem);

        public Task<CartItem?> UpdateCartItem(Guid id, CartItem patch);

        public IList<CartItem> GetCartItems(Guid userId);

        public CartItem? GetCartItemById(Guid id);

        public Task<bool> DeleteCartItem(Guid id);
    }
}
