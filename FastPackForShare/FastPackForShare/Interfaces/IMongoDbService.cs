using FastPackForShare.Default;

namespace FastPackForShare.Interfaces;

public interface IMongoDbService<TBaseEntityModel> : IDisposable where TBaseEntityModel : BaseEntityModel
{
    void SetConnectionString(string connectionString, string databaseName);
    Task CreateItem(TBaseEntityModel entity);
    Task UpdateItem(string propertyNameToFind, object valueToFind, string propertyNameToChange, object valueToChange);
    Task<bool> ResearchItem(string propertyNameToFind, object valueToFind);
    Task DeleteItem(string propertyNameToFind, object valueToFind);
}
