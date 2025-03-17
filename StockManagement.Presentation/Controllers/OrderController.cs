using Microsoft.AspNetCore.Mvc;
using StockManagement.Services.DTOs.Order;
using StockManagement.Services.Interfaces;
using System.Text;

namespace StockManagement.Presentation.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, IProductService productService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _productService = productService;
            _logger = logger;
        }

        // GET: Order
        public async Task<IActionResult> Index()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync();
                return View(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders");
                TempData["ErrorMessage"] = "We couldn't retrieve your orders at this time. Our team has been notified of this issue.";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Order/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                if (products == null || !products.Any())
                {
                    TempData["ErrorMessage"] = "You need to add products before creating an order. Please add at least one product first.";
                    return RedirectToAction("Index", "Product");
                }

                ViewBag.Products = products;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create order page");
                TempData["ErrorMessage"] = "We couldn't load the order creation page. Our team has been notified of this issue.";
                return RedirectToAction("Index");
            }
        }

        // POST: Order/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOrderDto createOrderDto)
        {
            try
            {
                // Log the received order data for debugging
                LogOrderData(createOrderDto);

                if (ModelState.IsValid)
                {
                    try
                    {
                        var orderDto = await _orderService.CreateOrderAsync(createOrderDto);
                        TempData["SuccessMessage"] = "Your order has been created successfully!";
                        return RedirectToAction(nameof(Details), new { id = orderDto.Id });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error creating order");

                        // More user-friendly error messages based on exception type
                        if (ex.Message.Contains("Insufficient stock"))
                        {
                            ModelState.AddModelError("", "Some items in your order have insufficient stock. Please adjust the quantities.");
                        }
                        else if (ex.Message.Contains("not found"))
                        {
                            ModelState.AddModelError("", "One or more products in your order are no longer available.");
                        }
                        else
                        {
                            ModelState.AddModelError("", "We couldn't process your order. Please try again or contact support if the issue persists.");
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

                    ModelState.AddModelError("", "Please check your order details and try again.");
                }

                var products = await _productService.GetAllProductsAsync();
                ViewBag.Products = products;
                return View(createOrderDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in Create action");
                TempData["ErrorMessage"] = "Something went wrong while processing your order. Our team has been notified of this issue.";
                return RedirectToAction("Index");
            }
        }

        // GET: Order/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);

                if (order == null)
                {
                    TempData["ErrorMessage"] = "The order you're looking for doesn't exist or has been removed.";
                    return RedirectToAction("Index");
                }

                return View(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order details for ID: {OrderId}", id);
                TempData["ErrorMessage"] = "We couldn't retrieve the order details. Our team has been notified of this issue.";
                return RedirectToAction("Index");
            }
        }

        // AJAX endpoint to get product details
        [HttpGet]
        public async Task<IActionResult> GetProductDetails(int productId)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(productId);
                if (product == null)
                {
                    return Json(new { error = "Product not found" });
                }

                return Json(new
                {
                    price = product.Price,
                    stockQuantity = product.StockQuantity
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product details for ID: {ProductId}", productId);
                return Json(new { error = "Unable to retrieve product information" });
            }
        }

        // Helper method to log order data for debugging
        private void LogOrderData(CreateOrderDto createOrderDto)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Received order data:");

            if (createOrderDto.OrderItems == null)
            {
                sb.AppendLine("OrderItems is null");
            }
            else
            {
                sb.AppendLine($"OrderItems count: {createOrderDto.OrderItems.Count}");

                for (int i = 0; i < createOrderDto.OrderItems.Count; i++)
                {
                    var item = createOrderDto.OrderItems[i];
                    sb.AppendLine($"Item {i}: ProductId={item.ProductId}, Quantity={item.Quantity}");
                }
            }

            _logger.LogInformation(sb.ToString());
        }
    }
}