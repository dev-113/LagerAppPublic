using LagerApp.Services;
using LagerApp.Views.Product;
using Microsoft.AspNetCore.Mvc;

namespace LagerApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly IDataService _dataService;
        private readonly ISqlRepository _sqlRepository;

        public ProductController(IDataService dataService, ISqlRepository sqlRepository)
        {
            _dataService = dataService;
            _sqlRepository = sqlRepository;
        }

        [HttpGet("")]
        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
                return View();

            var success = await _dataService.LoginAsync(loginVM);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Incorrect Username or Password");
                return View();
            }

            return RedirectToAction(nameof(AllProducts));
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            var result = await _dataService.LogoutAsync();
            if (!result) return BadRequest();

            return RedirectToAction(nameof(Login));
        }

        [HttpGet("signup")]
        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignupAsync(SignupVM signupVM)
        {
            if (!ModelState.IsValid) return View();
            var isErrorMessage = await _dataService.SignupAsync(signupVM);
            if (!string.IsNullOrWhiteSpace(isErrorMessage))
            {
                ModelState.AddModelError(string.Empty, isErrorMessage);
                return View();
            }

            // Set success message in TempData
            TempData["SuccessMessage"] = "Användare har lagts till!";

            return View();
        }

        [HttpGet("addproduct")]
        public IActionResult Product()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Product(ProductVM productVM)
        {
            if (!ModelState.IsValid) return View();
            var result = await _dataService.AddProduct(productVM);
            if (result)
            {
                return Json(new { success = "Product added successfully" });
            }
            else if (!result)
            {
                return Json(new { error = "Failed to add product, Product may already be in database" });
            }
            else
            {
                return BadRequest(new { error = "Failed to add product" });
            }


        }

        [HttpGet("Allproducts")]
        public async Task<IActionResult> AllProducts()
        {
            var products = await _dataService.GetProducts();
            return View(products);
        }

        [HttpGet("CheckArticleNumberAsync")]
        public async Task<IActionResult> CheckArticleNumberAsync(string articleNumber)
        {
            // Implement logic to check if the article number exists in the database
            // Return true if it exists, false otherwise
            var exists = await _sqlRepository.ArticleNumberExists(articleNumber);
            return Json(exists);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantityAsync(string articleNumber, string action)
        {
            var product = await _dataService.UpdateProductQuantity(articleNumber, action);
            // Redirect back to the product list after updating
            return View("AllProducts", product);
        }

        [HttpGet]
        public async Task<IActionResult> SearchProduct(string articleNumber)
        {
            if (string.IsNullOrWhiteSpace(articleNumber))
            {
                return View();
            }

            var product = await _dataService.SearchProduct(articleNumber);
            if (product == null)
            {
                return View();
            }
            return PartialView("_ProductTable", product);
        }

        [HttpGet]
        public async Task<IActionResult> ClearSearch()
        {
            var products = await _dataService.GetProducts();
            return PartialView("_ProductTable", products);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(string articleNumber)
        {
            var success = await _sqlRepository.DeleteProductAsync(articleNumber);
            // Return the updated product list
            var products = await _dataService.GetProducts();
            return PartialView("_ProductTable", products);
        }

        [HttpGet("storage")]
        public async Task<IActionResult> StorageCheck()
        {
            var products = await _dataService.GetProducts();
            var listProducts = await _dataService.GetProductsFromList();
            var storageCheckVM = new StorageCheckVM()
            {
                ProductVMs = products.ProductVMs,
                ListProducts = listProducts.ListProducts,
            };
            return View(storageCheckVM);
        }

        [HttpPost]
        public async Task<IActionResult> AddProductToList(string articleNumber, int quantity)
        {
            var result = await _sqlRepository.AddProductToList(articleNumber, quantity);
            if (!result) return View();

            // Fetch the updated list of scanned products and return the partial view
            var listProducts = await _dataService.GetProductsFromList();
            return PartialView("_ListProductsPartial", listProducts);
        }

        [HttpPost]
        public async Task<IActionResult> CopyListProductsToDatabase(string articleNumber, int quantity)
        {
            var result = await _dataService.CopyListProductsToDatabase();
            if (!result) return View();

            // Fetch the updated list of scanned products and return the partial view
            var databaseProducts = await _dataService.GetProducts();

            //remake to storageVM cause the model is used in _ListProductsPartial view.
            var storageVM = new StorageCheckVM()
            {
                ProductVMs = databaseProducts.ProductVMs
            };
            return PartialView("_DatabaseProductsPartial", storageVM);
        }

        [HttpPost]
        public async Task<IActionResult> ClearList()
        {
            var result = await _dataService.ClearListProducts();
            if (!result) return View();

            return PartialView("_ListProductsPartial", new StorageCheckVM());
        }

        [HttpPost]
        public async Task<IActionResult> CheckDiff()
        {
            var products = await _dataService.CheckDiff();
            var unmatchedProcuts = await _dataService.GetUnmatchedProductsToDiffList();
            var diffList = products.Concat(unmatchedProcuts).ToList();
            if (products.Count == 0) return View();

            var storageVM = new StorageCheckVM()
            {
                DiffProducts = diffList,
            };
            return PartialView("_DiffProductsPartial", storageVM);
        }

        [HttpPost]
        public async Task<IActionResult> ExportDatabaseExcel()
        {
            var result = await _dataService.ProductsFromDatabaseExcelWriter();
            if (!result) return Json(new { success = false, message = "Failed to export database" });

            return Json(new { success = true, message = "Database exported successfully" });
        }


        [HttpPost]
        public async Task<IActionResult> ExportListExcel()
        {
            var result = await _dataService.ProductsFromListExcelWriter();
            if (!result) return Json(new { success = false, message = "Failed to export list" });

            return Json(new { success = true, message = "List exported successfully" });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteListProduct(string articleNumber)
        {
            var success = await _sqlRepository.DeleteProductFromList(articleNumber);
            // Return the updated product list
            var products = await _dataService.GetProductsFromList();
            return RedirectToAction(nameof(StorageCheck));
        }
    }
}
