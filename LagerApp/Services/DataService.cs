using LagerApp.Models;
using LagerApp.Views.Product;
using Microsoft.AspNetCore.Identity;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using Microsoft.Extensions.Options;

namespace LagerApp.Services
{
    public class DataService : IDataService
    {
        private readonly EliasaphramSeDb7Context _context;
        private readonly ISqlRepository _sqlRepository;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<DataService> _logger;
        private readonly IExcelWriter _excelWriter;


        public DataService(EliasaphramSeDb7Context context, ISqlRepository sqlRepository, SignInManager<IdentityUser> signInManager, ILogger<DataService> logger, IExcelWriter excelWriter)
        {
            _context = context;
            _sqlRepository = sqlRepository;
            _signInManager = signInManager;
            _logger = logger;
            _excelWriter = excelWriter;
        }

        public async Task<string> SignupAsync(SignupVM signupVM)
        {
            var user = await _signInManager.UserManager.FindByNameAsync(signupVM.Username);
            if (user != null) _logger.LogInformation($"email adress already exists in database");
            var newUser = new IdentityUser()
            {
                UserName = signupVM.Username,
            };
            var createUser = await _signInManager.UserManager.CreateAsync(newUser, signupVM.Password);
            var result = createUser.Succeeded ? null : createUser.Errors.First().Description;
            return result;
        }

        public async Task<bool> LoginAsync(LoginVM loginVM)
        {
            var result = await _signInManager.PasswordSignInAsync(loginVM.Username, loginVM.Password, false, false);
            return result.Succeeded;
        }

        public async Task<bool> LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            return true;
        }

        public async Task<bool> AddProduct(ProductVM productVM)
        {
            var addProductOk = await _sqlRepository.AddProduct(productVM);
            if (addProductOk) return true;
            _logger.LogInformation($"failed to add product to db");
            return false;
        }

        public async Task<AllProductsVM> GetProducts()
        {
            var products = await _sqlRepository.GetProducts();
            if (products == null) return new AllProductsVM();


            var productVMs = new List<ProductVM>();
            foreach (var product in products)
            {
                productVMs.Add(new ProductVM()
                {
                    Id = product.Id,
                    ArticleNumber = product.ArticleNumber,
                    PurchasePrice = product.PurchasePrice,
                    SellingPrice = product.SellingPrice,
                    Weight = product.Weight,
                    Dimension = product.Dimension,
                    Material = product.Material,
                    Quantity = product.Quantity,
                });
            }
            return new AllProductsVM()
            {
                ProductVMs = productVMs
            };
        }

        public async Task<Product> GetProduct(string articleNumber)
        {
            var product = await _sqlRepository.GetProduct(articleNumber);
            if (product == null) return null;

            var newProduct = new Product()
            {
                ArticleNumber = product.ArticleNumber,
                PurchasePrice= product.PurchasePrice,
                SellingPrice= product.SellingPrice,
                Weight = product.Weight,
                Dimension = product.Dimension,
                Material = product.Material,
                Quantity = product.Quantity,
            };

            return newProduct;
        }

        public async Task<AllProductsVM> UpdateProductQuantity(string articleNumber, string action)
        {
            var product = await _sqlRepository.GetProduct(articleNumber);
            if (product == null) return new AllProductsVM();

            if (action == "increment")
            {
                product.Quantity += 1;
                await _context.SaveChangesAsync();
            }
            else
            {
                product.Quantity -= 1;
                await _context.SaveChangesAsync();
            }

            var productVM = new List<ProductVM>()
            {
                new ProductVM()
                {
                Id= product.Id,
                ArticleNumber = articleNumber,
                PurchasePrice = product.PurchasePrice,
                SellingPrice = product.SellingPrice,
                Weight = product.Weight,
                Dimension = product.Dimension ?? 0,
                Material = product.Material,
                Quantity = product.Quantity,
                }
            };
            return new AllProductsVM()
            {
                ProductVMs = productVM
            };
        }

        public async Task<AllProductsVM> SearchProduct(string articleNumber)
        {
            var product = await _sqlRepository.GetProduct(articleNumber);
            if (product == null) return null;

            var productVMs = new List<ProductVM>
            {
                new ProductVM()
                {
                    Id = product.Id,
                    ArticleNumber = product.ArticleNumber,
                    PurchasePrice = product.PurchasePrice,
                    SellingPrice = product.SellingPrice,
                    Weight = product.Weight,
                    Dimension = product.Dimension,
                    Material = product.Material,
                    Quantity = product.Quantity,
                }
            };
            return new AllProductsVM()
            {
                ProductFound = product != null,
                ProductVMs = productVMs
            };
        }

        public async Task<StorageCheckVM> GetProductsFromList()
        {
            var products = await _sqlRepository.GetProductsFromList();
            if (products == null) return new StorageCheckVM();

            return new StorageCheckVM()
            {
                ListProducts = products
            };
        }

        public async Task<bool> CopyListProductsToDatabase()
        {
            var listProducts = await _sqlRepository.GetProductsFromList();
            if (listProducts.Count == 0) return false;

            var databaseProducts = await _sqlRepository.GetProducts();
            if (databaseProducts.Count == 0) return false;

            var matchedProducts = GetMatchingProducts(databaseProducts, listProducts);

            foreach (var product in matchedProducts)
            {
                await _sqlRepository.UpdateProduct(product.ArticleNumber, product.Quantity);
            }
            return true;
        }

        private List<List> GetMatchingProducts(List<Product> databaseProducts, List<List> listProducts)
        {
            var matchedList = listProducts.Where(x => databaseProducts.Any(a => a.ArticleNumber == x.ArticleNumber)).ToList();
            return matchedList;
        }

        public async Task<List<List>> GetUnmatchedProductsToDiffList()
        {
            var dbProducts = await _sqlRepository.GetProducts();
            var listProducts = await _sqlRepository.GetProductsFromList();

            var unmatchedProducts = GetUnmatchedProducts(dbProducts, listProducts).Select(x => new List()
            {
                ArticleNumber = x.ArticleNumber,
                Quantity = x.Quantity,
            }).ToList();

            return unmatchedProducts;
        }

        private List<Product> GetUnmatchedProducts(List<Product> databaseProducts, List<List> listProducts)
        {
            var unmatchedList = databaseProducts.Where(dbProduct => !listProducts.Any(lp => lp.ArticleNumber == dbProduct.ArticleNumber)).ToList();
            return unmatchedList;
        }

        public async Task<bool> ClearListProducts()
        {
            var listProducts = await _sqlRepository.GetProductsFromList();
            if (listProducts.Count == 0) return false;
            foreach (var product in listProducts)
            {
                await _sqlRepository.DeleteProductFromList(product.ArticleNumber);
            }
            return true;
        }

        public async Task<List<List>> CheckDiff()
        {
            var listProducts = await _sqlRepository.GetProductsFromList();
            if (listProducts.Count == 0) return new List<List>();

            var databaseProducts = await _sqlRepository.GetProducts();
            if (databaseProducts.Count == 0) return new List<List>();

            var comparedResult = SubtractQuantities(databaseProducts, listProducts);
            if (comparedResult.Count == 0) return new List<List>();

            return comparedResult;
        }

        private List<List> SubtractQuantities(List<Product> databaseProducts, List<List> listProducts)
        {
            var resultList = new List<List>();

            foreach (var product in databaseProducts)
            {
                var matchingProduct = listProducts.FirstOrDefault(p => p.ArticleNumber == product.ArticleNumber);

                if (matchingProduct == null) continue;

                int updatedQuantity = product.Quantity - matchingProduct.Quantity;
                if (updatedQuantity != 0)
                {
                    resultList.Add(new List { ArticleNumber = product.ArticleNumber, Quantity = updatedQuantity });
                }
            }
            return resultList;
        }

        public async Task<bool> ProductsFromDatabaseExcelWriter()
        {
            try
            {
                var products = await _sqlRepository.GetProducts();
                if (products.Count == 0) return false;

                var result = await _excelWriter.ProductsFromDatabaseExcelWriter(products);
                if (!result) return false;

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> ProductsFromListExcelWriter()
        {
            try
            {
                var products = await _sqlRepository.GetProductsFromList();
                if (products.Count == 0) return false;

                var result = await _excelWriter.ProductsFromListExcelWriter(products);
                if (!result) return false;

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
