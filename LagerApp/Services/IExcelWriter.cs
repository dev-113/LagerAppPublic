using LagerApp.Models;

namespace LagerApp.Services
{
    public interface IExcelWriter
    {
        Task<bool> ProductsFromDatabaseExcelWriter(List<Product> products);
        Task<bool> ProductsFromListExcelWriter(List<List> products);
    }
}
