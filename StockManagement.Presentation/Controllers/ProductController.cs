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

        // GET: Product
        public async Task<IActionResult> Index()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return View(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products");
                TempData["ErrorMessage"] = "We couldn't retrieve the product list at this time. Our team has been notified of this issue.";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    TempData["ErrorMessage"] = $"The product you're looking for (ID: {id}) doesn't exist or has been removed.";
                    return RedirectToAction(nameof(Index));
                }
                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product details for ID: {ProductId}", id);
                TempData["ErrorMessage"] = "We couldn't retrieve the product details. Our team has been notified of this issue.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductDto createProductDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        var productDto = await _productService.CreateProductAsync(createProductDto);
                        TempData["SuccessMessage"] = "Your product has been created successfully!";
                        return RedirectToAction(nameof(Details), new { id = productDto.Id });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error creating product");

                        // More user-friendly error message
                        if (ex.Message.Contains("duplicate"))
                        {
                            ModelState.AddModelError("", "A product with this name already exists. Please use a different name.");
                        }
                        else
                        {
                            ModelState.AddModelError("", "We couldn't create your product. Please try again or contact support if the issue persists.");
                        }
                    }
                }
                else
                {
                    // Log model state errors for debugging
                    var errors = string.Join("; ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    _logger.LogWarning("Model validation failed: {Errors}", errors);

                    ModelState.AddModelError("", "Please check the product details and try again.");
                }
                return View(createProductDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in Create action");
                TempData["ErrorMessage"] = "Something went wrong while creating your product. Our team has been notified of this issue.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    TempData["ErrorMessage"] = $"The product you're trying to edit (ID: {id}) doesn't exist or has been removed.";
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product for edit, ID: {ProductId}", id);
                TempData["ErrorMessage"] = "We couldn't retrieve the product for editing. Our team has been notified of this issue.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateProductDto updateProductDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        await _productService.UpdateProductAsync(id, updateProductDto);
                        TempData["SuccessMessage"] = "Your product has been updated successfully!";
                        return RedirectToAction(nameof(Details), new { id });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error updating product with ID: {ProductId}", id);

                        // More user-friendly error message
                        if (ex.Message.Contains("not found"))
                        {
                            ModelState.AddModelError("", "The product you're trying to update no longer exists.");
                        }
                        else if (ex.Message.Contains("duplicate"))
                        {
                            ModelState.AddModelError("", "A product with this name already exists. Please use a different name.");
                        }
                        else
                        {
                            ModelState.AddModelError("", "We couldn't update your product. Please try again or contact support if the issue persists.");
                        }
                    }
                }
                else
                {
                    // Log model state errors for debugging
                    var errors = string.Join("; ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    _logger.LogWarning("Model validation failed: {Errors}", errors);

                    ModelState.AddModelError("", "Please check the product details and try again.");
                }
                return View(updateProductDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in Edit action for ID: {ProductId}", id);
                TempData["ErrorMessage"] = "Something went wrong while updating your product. Our team has been notified of this issue.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    TempData["ErrorMessage"] = $"The product you're trying to delete (ID: {id}) doesn't exist or has been removed.";
                    return RedirectToAction(nameof(Index));
                }
                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product for delete, ID: {ProductId}", id);
                TempData["ErrorMessage"] = "We couldn't retrieve the product for deletion. Our team has been notified of this issue.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                try
                {
                    await _productService.DeleteProductAsync(id);
                    TempData["SuccessMessage"] = "Your product has been deleted successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error deleting product with ID: {ProductId}", id);

                    // More user-friendly error message based on exception type
                    if (ex.Message.Contains("used in"))
                    {
                        ModelState.AddModelError("", ex.Message);
                    }
                    else
                    {
                        ModelState.AddModelError("", "We couldn't delete this product. Please try again or contact support if the issue persists.");
                    }

                    var product = await _productService.GetProductByIdAsync(id);
                    return View(product);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in DeleteConfirmed action for ID: {ProductId}", id);
                TempData["ErrorMessage"] = "Something went wrong while deleting your product. Our team has been notified of this issue.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}