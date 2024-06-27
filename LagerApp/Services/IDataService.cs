using LagerApp.Models;
using LagerApp.Views.Product;

namespace LagerApp.Services
{
    public interface IDataService
    {
        Task<string> SignupAsync(SignupVM signupVM);
        Task<bool> LoginAsync(LoginVM loginVM);
        Task<bool> LogoutAsync();
        Task<bool> AddProduct(ProductVM productVM);
        Task<AllProductsVM> GetProducts();
        Task<Product> GetProduct(string articleNumber);
        Task<AllProductsVM> UpdateProductQuantity(string articleNumber, string action);
        Task<AllProductsVM> SearchProduct(string articleNumber);
        Task<StorageCheckVM> GetProductsFromList();
        Task<bool> CopyListProductsToDatabase();
        Task<bool> ClearListProducts();
        Task<List<List>> CheckDiff();
        Task<List<List>> GetUnmatchedProductsToDiffList();
        Task<bool> ProductsFromDatabaseExcelWriter();
        Task<bool> ProductsFromListExcelWriter();
    }
}
