using StockManagement.Core.Interfaces;
using StockManagement.Core.Interfaces.Repositories;
using StockManagement.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockManagement.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;

            Orders = new OrderRepository(context);
            Products = new ProductRepository(context);

        }

        public IOrderRepository Orders { get; private set; }
        public IProductRepository Products { get; private set; }
        public async Task Complete()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
