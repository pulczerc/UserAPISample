using System.Linq;
using System.Threading.Tasks;
using UserAPISample.Dal.Interfaces;
using UserAPISample.Model;
using MongoDB.Driver;

namespace UserAPISample.Dal.Implementations
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private static readonly object lockMethod = new object();

        public UserRepository(IMongoDBContext context) : base(context)
        {
        }

        public new Task<User> InsertAsync(User entity)
        {
            entity.Id = NextIdNumber();
            lock (lockMethod)
            {
                _dbCollection.InsertOne(entity);
            }
            return Task.FromResult(entity);
        }

        public override Task RemoveAsync(User entity) =>
            _dbCollection.DeleteOneAsync(u => u.Id == entity.Id);

        /// <summary>
        /// Generate the next increment id
        /// </summary>
        /// <returns>id value</returns>
        public virtual int NextIdNumber()
        {
            lock (lockMethod)
            {
                var lastUser = _dbCollection.Find(Builders<User>.Filter.Empty).SortByDescending(u => u.Id).Limit(1).FirstOrDefault();

                return (lastUser?.Id ?? 0) + 1;
            }
        }
    }
}
