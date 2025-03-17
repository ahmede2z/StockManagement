using AutoMapper;
using StockManagement.Core.Entities;
using StockManagement.Core.Interfaces;
using StockManagement.Services.DTOs.Product;
using StockManagement.Services.Interfaces;
using System.Linq;

namespace StockManagement.Services.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto> GetProductByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            return product == null ? null : _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
        {
            var product = _mapper.Map<Product>(createProductDto);
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.Complete();
            return _mapper.Map<ProductDto>(product);
        }

        public async Task UpdateProductAsync(int id, UpdateProductDto updateProductDto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
                throw new Exception("Product not found.");
            _mapper.Map(updateProductDto, product);
            _unitOfWork.Products.Update(product);
            await _unitOfWork.Complete();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
                throw new Exception("Product not found.");

            // Get all order items associated with this product
            var orderItems = await _unitOfWork.Orders.GetOrderItemsByProductIdAsync(id);

            // If there are order items, delete them first
            if (orderItems.Any())
            {
                // Group order items by order ID to identify affected orders
                var affectedOrderIds = orderItems.Select(oi => oi.OrderId).Distinct().ToList();

                // Delete all order items associated with this product
                foreach (var orderItem in orderItems)
                {
                    _unitOfWork.Orders.DeleteOrderItem(orderItem);
                }

                // Check if any orders are now empty (no items) and delete them too
                foreach (var orderId in affectedOrderIds)
                {
                    var order = await _unitOfWork.Orders.GetOrderWithItemsAsync(orderId);
                    if (order != null && !order.OrderItems.Any(oi => oi.ProductId != id))
                    {
                        _unitOfWork.Orders.Delete(order);
                    }
                }
            }

            // Now delete the product
            _unitOfWork.Products.Delete(product);
            await _unitOfWork.Complete();
        }
    }
}
