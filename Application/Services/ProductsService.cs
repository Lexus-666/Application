using kursah_5semestr.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace kursah_5semestr.Services
{
    public class ProductsService : IProductsService
    {
        private AppDbContext _context;

        public ProductsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Product> CreateProduct(Product product)
        {
            product.Id = Guid.NewGuid();
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
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
                        return product;
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
                return true;
            }
            else { return false; }
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
                }
            }
        }
    }
}