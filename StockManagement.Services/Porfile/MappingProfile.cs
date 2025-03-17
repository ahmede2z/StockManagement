using AutoMapper;
using StockManagement.Core.Entities;
using StockManagement.Services.DTOs.Order;
using StockManagement.Services.DTOs.Product;

namespace StockManagement.Services.Porfile
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDto>();
            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>();
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));
            CreateMap<CreateOrderItemDto, OrderItem>()
                .ForMember(dest => dest.UnitPrice, opt => opt.Ignore()); // Set in service
        }
    }
}
