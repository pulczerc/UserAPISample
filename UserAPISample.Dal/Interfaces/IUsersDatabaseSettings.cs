namespace UserAPISample.Dal.Interfaces
{
    public interface IUsersDatabaseSettings
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
        string UsersCollectionName { get; set; }
    }
}