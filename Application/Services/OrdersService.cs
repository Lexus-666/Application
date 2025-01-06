using kursah_5semestr.Abstractions;
using kursah_5semestr.Contracts;
using Microsoft.EntityFrameworkCore;

namespace kursah_5semestr.Services
{
    public class OrdersService : IOrdersService
    {
        private AppDbContext _context;
        private IBrokerService _brokerService;
        private IProductsService _productsService;
        private ILogger _logger;

        public OrdersService(AppDbContext context, IBrokerService brokerService, IProductsService productsService, ILogger<OrdersService> logger)
        {
            _context = context;
            _brokerService = brokerService;
            _productsService = productsService;
            _logger = logger;
        }

        public async Task<Order> CreateOrder(User user, OrderStatus status, IList<CartItem> cartItems)
        {
            //calculate total amount
            double totalAmount = cartItems.Sum(
                item => item.Product.Price.GetValueOrDefault() * item.Quantity.GetValueOrDefault()
                );

            var orderId = Guid.NewGuid();

            //order details generator
            var orderDetails = cartItems.Select(item => new OrderDetails(
                orderId,
                item.Product,
                item.Quantity.GetValueOrDefault(),
                item.Product.Price.GetValueOrDefault()
            )).ToList();

            var (order, error) = Order.Create(orderId, user, status, totalAmount, cartItems, orderDetails);

            if (!string.IsNullOrEmpty(error))
            {
                _logger.LogError($"Error creating order: {error}");
                throw new InvalidOperationException(error);
            }

            var (success, insufficientStocks) = await _productsService.ProcessCreateOrder(order!);
            if (!success)
            {
                var someProductsWereNotFound = insufficientStocks.Where(p => p == null).Count() > 0;
                var productWithInsuffStocks = insufficientStocks.Where(p => p != null);
                var message = "";
                if (someProductsWereNotFound)
                {
                    message += "Some products were not found";
                }
                if (productWithInsuffStocks.Count() > 0)
                {
                    if (someProductsWereNotFound)
                    {
                        message += "; ";
                    }
                    message += "Insufficient stocks: ";
                    var names = string.Join(", ", productWithInsuffStocks.Select(p => p.Title));
                    message += names;
                    _logger.LogError($"Error creating order: {message}");
                    throw new InvalidOperationException(message);
                }
            }
            _context.Orders.Add(order!);
            await _context.SaveChangesAsync();
            return order!;
        }

        public async Task<Order> CreateOrder(User user)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var order = await CreateOrder(user, OrderStatus.New, [.. user.CartItems]);
                    user.CartItems = [];
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                    var dto = new OrderOutDto(
                        Id: order.Id, 
                        Date: order.Date, 
                        Status: order.Status, 
                        Amount: order.Amount, 
                        OrderDetails: [], 
                        UserId: order.UserId);
                    _logger.LogInformation($"Created order {order.Id}");
                    var message = new InstanceChangedOut(Action: "create", Entity: "order", Data: dto);
                    await _brokerService.SendMessage("changes", message);
                    return order;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.LogError(ex, "Error creating order");
                    throw;
                }
            }
        }


        public async Task<Order?> UpdateOrderStatus(Guid id, OrderStatus newStatus)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                _logger.LogWarning($"Order {id} not found");
                return null;
            }

            order.Status = newStatus;
            _context.Orders.Update(order);
            _logger.LogInformation($"Updated status of the order {id}");
            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<Order?> GetOrderById(Guid id)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IList<Order>> GetOrdersByUser(Guid userId)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Where(o => o.UserId == userId)
                .ToListAsync();
        }

        public async Task<IList<Order>> GetAllOrders()
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Include(o => o.User)
                .ToListAsync();
        }

        public async Task<bool> DeleteOrder(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                _logger.LogWarning($"Order {id} not found");
                return false;
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await _productsService.ProcessDeleteOrder(order);
                    _context.Orders.Remove(order);
                    _logger.LogInformation($"Deleted order {id}");
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.LogError(ex, $"Error deleting order {id}");
                    throw;
                }
                return true;
            }
        }
    }
}
