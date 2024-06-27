using LagerApp.Models;

namespace LagerApp.Views.Product
{
    public class StorageCheckVM
    {
        public List<ProductVM> ProductVMs { get; set; }
        public List<List> ListProducts { get; set; }
        public List<List> DiffProducts { get; set; }
    }
}
