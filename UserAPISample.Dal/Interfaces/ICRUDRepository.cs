using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAPISample.Dal.Interfaces
{
    /// <summary>
    /// Supports CRUD operations
    /// </summary>
    /// <typeparam name="T">Any Model class</typeparam>
    public interface ICRUDRepository<T> where T : class
    {
        /// <summary>
        /// Insert new entity
        /// </summary>
        /// <param name="entity">Entity</param>
        Task<T> InsertAsync(T entity);

        /// <summary>
        /// Get all entities
        /// </summary>
        /// <returns>List of entities</returns>
        Task<IEnumerable<T>> GetAsync();

        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        Task<T> GetAsync(int id);

        /// <summary>
        /// Delete entity by Identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        Task RemoveAsync(int id);

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity">Entity</param>
        Task RemoveAsync(T entity);

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        Task UpdateAsync(int id, T entity);

        /// <summary>
        /// Gets a Collection
        /// </summary>
        IQueryable<T> Collection { get; }
    }
}
