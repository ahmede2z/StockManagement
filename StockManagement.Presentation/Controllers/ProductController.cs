using Microsoft.AspNetCore.Mvc;
using StockManagement.Services.DTOs.Product;
using StockManagement.Services.Interfaces;

namespace StockManagement.Presentation.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                TempData["ErrorMessage"] = "The requested product could not be found.";
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductDto createProductDto)
        {
            if (!ModelState.IsValid)
                return View(createProductDto);

            var productDto = await _productService.CreateProductAsync(createProductDto);
            TempData["SuccessMessage"] = "Product created successfully.";
            return RedirectToAction(nameof(Details), new { id = productDto.Id });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                TempData["ErrorMessage"] = "The requested product could not be found.";
                return RedirectToAction(nameof(Index));
            }

            var updateProductDto = new UpdateProductDto
            {
                Name = product.Name,
                Price = product.Price,
                StockQuantity = product.StockQuantity
            };

            return View(updateProductDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateProductDto updateProductDto)
        {
            if (!ModelState.IsValid)
                return View(updateProductDto);

            await _productService.UpdateProductAsync(id, updateProductDto);
            TempData["SuccessMessage"] = "Product updated successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                TempData["ErrorMessage"] = "The requested product could not be found.";
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productService.DeleteProductAsync(id);
            TempData["SuccessMessage"] = "Product deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}