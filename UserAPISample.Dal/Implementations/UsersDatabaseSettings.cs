using UserAPISample.Dal.Interfaces;

namespace UserAPISample.Dal.Implementations
{
    public class UsersDatabaseSettings : IUsersDatabaseSettings
    {
        public string UsersCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
