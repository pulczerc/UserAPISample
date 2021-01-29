using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAPISample.Dal.Interfaces;

namespace UserAPISample.Dal.Implementations
{
    public class MongoDBContext : IMongoDBContext
    {
        private readonly IMongoDatabase _db;
        private readonly MongoClient _mongoClient;
        private readonly string _collectionName;

        public MongoDBContext(IUsersDatabaseSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            //MongoDB .NET Driver API Documentation: https://api.mongodb.com/csharp/2.2/html/R_Project_CSharpDriverDocs.htm#!

            _mongoClient = new MongoClient(settings.ConnectionString);
            _db = _mongoClient.GetDatabase(settings.DatabaseName);
            _collectionName = settings.UsersCollectionName;
        }

        public IMongoCollection<T> GetCollection<T>()
        {
            return _db.GetCollection<T>(_collectionName);
        }
    }
}
