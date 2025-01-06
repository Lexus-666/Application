using kursah_5semestr.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace kursah_5semestr.Services
{
    public class ProductsService : IProductsService
    {
        private AppDbContext _context;
        private ILogger _logger;

        public ProductsService(AppDbContext context, ILogger<ProductsService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Product> CreateProduct(Product product)
        {
            product.Id = Guid.NewGuid();
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Created product {product.Id}");
            return product;
        }

        public async Task<Product?> UpdateProduct(Guid id, Product patch)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var product = GetProductById(id);
                    if (product != null)
                    {
                        if (patch.Title != null)
                        {
                            product.Title = patch.Title;
                        }
                        if (patch.Description != null)
                        {
                            product.Description = patch.Description;
                        }
                        if (patch.Price != null)
                        {
                            product.Price = (double)patch.Price;
                        }
                        if (patch.Quantity != null)
                        {
                            product.Quantity = (int)patch.Quantity;
                        }
                        _context.Products.Update(product);
                        await _context.SaveChangesAsync();
                        transaction.Commit();
                        _logger.LogInformation($"Updated product {id}");
                        return product;
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.LogError(ex, $"Error updating product {id}");
                    return null;
                }
            }
            return null;
        }

        public IList<Product> GetProducts()
        {
            return _context.Products.ToList();
        }

        public Product? GetProductById(Guid id)
        {
            return _context.Products.Find(id);
        }

        public async Task<bool> DeleteProduct(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Deleted product {id}");
                return true;
            }
            else {
                _logger.LogError($"Error deleting product {id}");
                return false; 
            }
        }

        public async Task<(bool success, IList<Product?> insufficientStocks)> ProcessCreateOrder(Order order)
        {
            var insufficientStocks = order.OrderDetails
                .Select(od => (od, GetProductById(od.ProductId)))
                .Where(odp => odp.Item2 == null || odp.Item2.Quantity < odp.Item1.Quantity)
                .Select(odp => odp.Item2)
                .ToList();
            if (insufficientStocks.Count > 0)
            {
                return (false, insufficientStocks);
            }
            foreach (var od in order.OrderDetails)
            {
                Product? product = GetProductById(od.ProductId);
                if (product != null)
                {
                    product.Quantity -= od.Quantity;
                    _context.Products.Update(product);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Decreased stock for product {product.Id} (order {order.Id})");
                }
            }
            return (true, []);
        }

        public async Task ProcessDeleteOrder(Order order)
        {
            foreach (var od in order.OrderDetails)
            {
                Product? product = GetProductById(od.ProductId);
                if (product != null)
                {
                    product.Quantity += od.Quantity;
                    _context.Products.Update(product);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Returned stock for product {product.Id} (order {order.Id})");
                }
            }
        }
    }
}