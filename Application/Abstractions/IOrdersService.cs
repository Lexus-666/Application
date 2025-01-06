namespace kursah_5semestr.Abstractions
{
    public interface IOrdersService
    {
        public Task<Order> CreateOrder(User user, OrderStatus status, IList<CartItem> cartItems);

        public Task<Order> CreateOrder(User user);

        public Task<Order?> UpdateOrderStatus(Guid id, OrderStatus status);

        public Task<Order?> GetOrderById(Guid id);

        public Task<IList<Order>> GetOrdersByUser(Guid user);

        public Task<IList<Order>> GetAllOrders();

        public Task<bool> DeleteOrder(Guid id);
    }
}
