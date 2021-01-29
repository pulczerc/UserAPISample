using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserAPISample.Dal.Interfaces;

namespace UserAPISample.Dal.Implementations
{
    public abstract class BaseRepository<T> : ICRUDRepository<T> where T : class
    {
        protected readonly IMongoDBContext _mongoContext;
        protected IMongoCollection<T> _dbCollection;

        protected BaseRepository(IMongoDBContext context)
        {
            _mongoContext = context ?? throw new ArgumentNullException(nameof(context));
            _dbCollection = _mongoContext.GetCollection<T>();
        }

        public IQueryable<T> Collection => _dbCollection.AsQueryable();

        public virtual Task<T> InsertAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(typeof(T).Name);
            _dbCollection.InsertOneAsync(entity);
            return Task.FromResult(entity);
        }

        public virtual async Task<IEnumerable<T>> GetAsync() =>
            await (await _dbCollection.FindAsync(Builders<T>.Filter.Empty).ConfigureAwait(false)).ToListAsync().ConfigureAwait(false);

        public virtual Task<T> GetAsync(int id) =>
            _dbCollection.FindAsync(Builders<T>.Filter.Eq("_id", id)).Result.FirstOrDefaultAsync();

        public virtual Task UpdateAsync(int id, T entity) =>
            _dbCollection.ReplaceOneAsync(Builders<T>.Filter.Eq("_id", id), entity);

        public virtual Task RemoveAsync(int id) =>
            _dbCollection.DeleteOneAsync(Builders<T>.Filter.Eq("_id", id));

        public abstract Task RemoveAsync(T entity);
    }
}
