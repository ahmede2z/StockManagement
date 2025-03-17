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
            return _mapper.Map<IEnumerable<ProductDto>>(products.Where(p => !p.IsDeleted));
        }

        public async Task<ProductDto> GetProductByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null || product.IsDeleted)
                return null;
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
        {
            var product = _mapper.Map<Product>(createProductDto);
            product.IsDeleted = false;
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.Complete();
            return _mapper.Map<ProductDto>(product);
        }

        public async Task UpdateProductAsync(int id, UpdateProductDto updateProductDto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null || product.IsDeleted)
                throw new Exception("Product not found.");
            _mapper.Map(updateProductDto, product);
            _unitOfWork.Products.Update(product);
            await _unitOfWork.Complete();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null || product.IsDeleted)
                throw new Exception("Product not found.");

            product.IsDeleted = true;
            _unitOfWork.Products.Update(product);
            await _unitOfWork.Complete();
        }
    }
}
