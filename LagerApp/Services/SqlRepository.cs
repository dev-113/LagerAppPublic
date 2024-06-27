using LagerApp.Models;
using LagerApp.Views.Product;
using Microsoft.EntityFrameworkCore;

namespace LagerApp.Services
{
    public class SqlRepository : ISqlRepository
    {
        private readonly EliasaphramSeDb7Context _context;
        private readonly ILogger<SqlRepository> _logger;

        public SqlRepository(EliasaphramSeDb7Context context, ILogger<SqlRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> AddProduct(ProductVM productVM)
        {
            try
            {
                var exist = await ArticleNumberExists(productVM.ArticleNumber);
                if (exist)
                {
                    _logger.LogInformation("article number already exists");
                    return false;
                }

                var product = new Product()
                {
                    ArticleNumber = productVM.ArticleNumber,
                    PurchasePrice = productVM.PurchasePrice,
                    SellingPrice = productVM.SellingPrice,
                    Weight = productVM.Weight,
                    Dimension = productVM.Dimension,
                    Material = productVM.Material,
                    Quantity = productVM.Quantity
                };
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                _logger.LogError("failed to add product");
                return false;
            }
        }

        public async Task<bool> DeleteProductAsync(string articleNumber)
        {
            try
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(x => x.ArticleNumber == articleNumber);

                if (product == null) return false;
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                _logger.LogError("failed to delete product");
                return false;
            }
        }

        public async Task<bool> UpdateProduct(string articleNumber, int quantity)
        {
            try
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(x => x.ArticleNumber == articleNumber);

                if (product == null) return false;
                product.Quantity = quantity;
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                _logger.LogError("failed to update product");
                return false;
            }
        }

        public async Task<List<Product>> GetProducts()
        {
            try
            {
                var products = await _context.Products
                    .ToListAsync();
                if (products == null) return new List<Product>();

                return products;
            }
            catch (Exception)
            {
                _logger.LogError("failed to get products");
                return new List<Product>();
            }
        }

        public async Task<Product> GetProduct(string articleNumber)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(x => x.ArticleNumber == articleNumber);
            if (product == null) return null;

            return product;
        }

        public async Task<bool> ArticleNumberExists(string articleNumber)
        {
            var exist = await _context.Products
                .AnyAsync(x => x.ArticleNumber == articleNumber);
            if (!exist) return false;
            return true;
        }

        public async Task<bool> ArticleNumberExistsList(string articleNumber)
        {
            var exist = await _context.Lists
                .AnyAsync(x => x.ArticleNumber == articleNumber);
            if (!exist) return false;
            return true;
        }

        public async Task<bool> AddProductToList(string articleNumber, int quantity)
        {
            try
            {
                var exist = await ArticleNumberExistsList(articleNumber);
                if (exist)
                {
                    _logger.LogInformation("article number already exists");
                    return false;
                }

                var product = new List()
                {
                    ArticleNumber = articleNumber,
                    Quantity = quantity
                };
                await _context.Lists.AddAsync(product);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                _logger.LogError("failed to add product");
                return false;
            }
        }

        public async Task<bool> DeleteProductFromList(string articleNumber)
        {
            try
            {
                var product = await _context.Lists
                    .FirstOrDefaultAsync(x => x.ArticleNumber == articleNumber);

                if (product == null) return false;
                _context.Lists.Remove(product);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                _logger.LogError("failed to delete product");
                return false;
            }
        }

        public async Task<List<List>> GetProductsFromList()
        {
            try
            {
                var products = await _context.Lists
                    .ToListAsync();
                if (products == null) return new List<List>();

                return products;
            }
            catch (Exception)
            {
                _logger.LogError("failed to get products");
                return new List<List>();
            }
        }
    }
}
