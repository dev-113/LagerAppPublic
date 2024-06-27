using LagerApp.Models;
using LagerApp.Views.Product;

namespace LagerApp.Services
{
    public interface ISqlRepository
    {
        Task<bool> AddProduct(ProductVM productVM);
        Task<bool> DeleteProductAsync(string articleNumber);
        Task<bool> UpdateProduct(string articleNumber, int quantity);
        Task<List<Product>> GetProducts();
        Task<bool> ArticleNumberExists(string articleNumber);
        Task<bool> ArticleNumberExistsList(string articleNumber);
        Task<Product> GetProduct(string articleNumber);
        Task<bool> AddProductToList(string articleNumber, int quantity);
        Task<bool> DeleteProductFromList(string articleNumber);
        Task<List<List>> GetProductsFromList();
    }
}
