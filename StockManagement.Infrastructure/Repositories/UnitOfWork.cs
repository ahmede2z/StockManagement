using Microsoft.EntityFrameworkCore;
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
        private bool _disposed;

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
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new Exception("The data you are trying to save has been modified by another user. Please refresh and try again.");
            }
            catch (Exception)
            {
                throw new Exception("An error occurred while saving changes. Please try again.");
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context.Dispose();
                _disposed = true;
            }
        }
    }
}
