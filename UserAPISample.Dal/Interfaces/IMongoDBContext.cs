using MongoDB.Driver;

namespace UserAPISample.Dal.Interfaces
{
    public interface IMongoDBContext
    {
        IMongoCollection<T> GetCollection<T>();
    }
}
