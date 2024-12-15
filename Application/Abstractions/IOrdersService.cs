namespace kursah_5semestr.Abstractions
{
    public interface IOrdersService
    {
        public Task<Order> CreateOrder(User user, string status, IList<CartItem> cartItems);

        public Task<Order> CreateOrder(User user);

        public Task<Order?> UpdateOrderStatus(Guid id, string status);

        public Task<Order?> GetOrderById(Guid id);

        public Task<IList<Order>> GetOrdersByUser(Guid user);

        public Task<IList<Order>> GetAllOrders();

        public Task<bool> DeleteOrder(Guid id);
    }
}
