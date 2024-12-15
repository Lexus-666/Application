namespace kursah_5semestr.Abstractions
{
    public interface IProductsService
    {
        public Task<Product> CreateProduct(Product product);

        public Task<Product?> UpdateProduct(Guid id, Product patch);

        public IList<Product> GetProducts();

        public Product? GetProductById(Guid id);

        public Task<bool> DeleteProduct(Guid id);

        public Task<(bool success, IList<Product?> insufficientStocks)> ProcessCreateOrder(Order order);

        public Task ProcessDeleteOrder(Order order);
    }
}
