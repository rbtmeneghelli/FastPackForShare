using FastPackForShare.Bases.Generics;
using FastPackForShare.Default;

namespace FastPackForShare.Interfaces;

public interface IMongoDbService<TGenericEntityModel> : IDisposable where TGenericEntityModel : GenericEntityModel
{
    void SetConnectionString(string connectionString, string databaseName);
    Task CreateItem(TGenericEntityModel entity);
    Task UpdateItem(string propertyNameToFind, object valueToFind, string propertyNameToChange, object valueToChange);
    Task<bool> ResearchItem(string propertyNameToFind, object valueToFind);
    Task DeleteItem(string propertyNameToFind, object valueToFind);
}
