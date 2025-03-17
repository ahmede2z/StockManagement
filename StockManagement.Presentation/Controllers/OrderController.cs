using Microsoft.AspNetCore.Mvc;
using StockManagement.Services.DTOs.Order;
using StockManagement.Services.Interfaces;

namespace StockManagement.Presentation.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;

        public OrderController(IOrderService orderService, IProductService productService)
        {
            _orderService = orderService;
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return View(orders);
        }

        public async Task<IActionResult> Create()
        {
            var products = await _productService.GetAllProductsAsync();
            if (!products.Any())
            {
                TempData["ErrorMessage"] = "No products available. Please add products before creating an order.";
                return RedirectToAction("Index", "Product");
            }

            ViewBag.Products = products;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOrderDto createOrderDto)
        {
            if (!ModelState.IsValid)
            {
                var products = await _productService.GetAllProductsAsync();
                ViewBag.Products = products;
                return View(createOrderDto);
            }

            var orderDto = await _orderService.CreateOrderAsync(createOrderDto);
            TempData["SuccessMessage"] = "Order created successfully.";
            return RedirectToAction(nameof(Details), new { id = orderDto.Id });
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                TempData["ErrorMessage"] = "The requested order could not be found.";
                return RedirectToAction("Index");
            }

            return View(order);
        }

        
    }
}