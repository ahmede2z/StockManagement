using Microsoft.EntityFrameworkCore;
using StockManagement.Core.Entities;
using StockManagement.Core.Interfaces.Repositories;
using StockManagement.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StockManagement.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all entities with optional includes
        public async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();
            query = ApplyIncludes(query, includes);
            return await query.AsNoTracking().ToListAsync();
        }

        // Get an entity by ID
        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        // Find a single entity based on criteria with optional includes
        public async Task<T> FindAsync(Expression<Func<T, bool>> criteria, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();
            query = ApplyIncludes(query, includes);
            return await query.SingleOrDefaultAsync(criteria);
        }

        // Find multiple entities based on criteria with optional includes
        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>().Where(criteria);
            query = ApplyIncludes(query, includes);
            return await query.AsNoTracking().ToListAsync();
        }

        // Query with pagination, sorting, and optional includes
        public async Task<IEnumerable<T>> QueryAsync(
            Expression<Func<T, bool>> criteria = null,
            int? skip = null,
            int? take = null,
            Expression<Func<T, object>> orderBy = null,
            string orderByDirection = "ASC",
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();

            if (criteria != null)
                query = query.Where(criteria);

            query = ApplyIncludes(query, includes);

            if (orderBy != null)
                query = orderByDirection == "ASC" ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);

            if (skip.HasValue)
                query = query.Skip(skip.Value);

            if (take.HasValue)
                query = query.Take(take.Value);

            return await query.AsNoTracking().ToListAsync();
        }

        // Add a new entity
        public async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            return entity; // SaveChanges will be handled by UoF
        }

        // Add a range of entities
        public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
            return entities; // SaveChanges will be handled by UoF
        }

        // Update an existing entity
        public T Update(T entity)
        {
            _context.Set<T>().Update(entity);
            return entity; // SaveChanges will be handled by UoF
        }

        // Delete an entity
        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        // Delete a range of entities
        public void DeleteRange(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
        }

        // Count all entities
        public async Task<int> CountAsync()
        {
            return await _context.Set<T>().CountAsync();
        }

        // Count entities matching criteria
        public async Task<int> CountAsync(Expression<Func<T, bool>> criteria)
        {
            return await _context.Set<T>().CountAsync(criteria);
        }

        // Helper: Apply includes to query
        private IQueryable<T> ApplyIncludes(IQueryable<T> query, params Expression<Func<T, object>>[] includes)
        {
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return query;
        }
    }
}
