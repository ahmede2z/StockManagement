using AutoMapper;
using StockManagement.Core.Entities;
using StockManagement.Core.Interfaces;
using StockManagement.Services.DTOs.Order;
using StockManagement.Services.Interfaces;

namespace StockManagement.Services.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _unitOfWork.Orders.GetOrdersWithItemsAsync();
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto> GetOrderByIdAsync(int id)
        {
            var order = await _unitOfWork.Orders.GetOrderWithItemsAsync(id);
            if (order == null)
                return null;

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto)
        {
            try
            {
                var order = new Order
                {
                    OrderDate = DateTime.Now,
                    OrderItems = new List<OrderItem>()
                };

                decimal totalAmount = 0;

                foreach (var item in createOrderDto.OrderItems)
                {
                    var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                    if (product == null)
                        throw new Exception($"Product with ID {item.ProductId} not found.");
                    if (product.StockQuantity < item.Quantity)
                        throw new Exception($"Insufficient stock for {product.Name}. Available: {product.StockQuantity}, Requested: {item.Quantity}");

                    var orderItem = new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price
                    };

                    order.OrderItems.Add(orderItem);
                    product.StockQuantity -= item.Quantity;
                    totalAmount += item.Quantity * product.Price;
                }

                order.TotalAmount = totalAmount;
                await _unitOfWork.Orders.AddAsync(order);
                await _unitOfWork.Complete();

                return _mapper.Map<OrderDto>(order);
            }
            catch (Exception ex)
            {
                throw new Exception($"Order creation failed: {ex.Message}");
            }
        }
    }
}
