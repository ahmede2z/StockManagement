using StockManagement.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StockManagement.Core.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        // Get an entity by ID
        Task<T> GetByIdAsync(int id);

        // Get all entities with optional includes
        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);

        // Find a single entity based on criteria with optional includes
        Task<T> FindAsync(Expression<Func<T, bool>> criteria, params Expression<Func<T, object>>[] includes);

        // Find multiple entities based on criteria with optional includes
        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, params Expression<Func<T, object>>[] includes);

        // Query with optional criteria, pagination, sorting, and includes
        Task<IEnumerable<T>> QueryAsync(
            Expression<Func<T, bool>> criteria = null,
            int? skip = null,
            int? take = null,
            Expression<Func<T, object>> orderBy = null,
            string orderByDirection = "ASC",
            params Expression<Func<T, object>>[] includes);

        // Add a new entity
        Task<T> AddAsync(T entity);

        // Add a range of entities
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);

        // Update an existing entity
        T Update(T entity);

        // Delete an entity
        void Delete(T entity);

        // Delete a range of entities
        void DeleteRange(IEnumerable<T> entities);

        // Count all entities
        Task<int> CountAsync();

        // Count entities matching criteria
        Task<int> CountAsync(Expression<Func<T, bool>> criteria);
    }
}
