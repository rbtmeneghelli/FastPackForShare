using System.Linq.Expressions;
using FastPackForShare.Bases.Generics;
using FastPackForShare.Default;
using FastPackForShare.Interfaces;
using FastPackForShare.Services.Bases;
using MongoDB.Driver;

namespace FastPackForShare.Services;

public sealed class MongoDbService<TGenericEntityModel> : BaseHandlerService, IMongoDbService<TGenericEntityModel> where TGenericEntityModel : GenericEntityModel
{
    private string _connectionString;
    private string _databaseName;

    public MongoDbService(INotificationMessageService iNotificationMessageService) : base(iNotificationMessageService)
    {
    }

    public void SetConnectionString(string connectionString, string databaseName)
    {
        _connectionString = connectionString;
        _databaseName = databaseName;
    }

    private IMongoCollection<TGenericEntityModel> CreateMongoDbConnection()
    {
        var client = new MongoClient(_connectionString);
        var database = client.GetDatabase(_databaseName);
        var collection = database.GetCollection<TGenericEntityModel>(nameof(TGenericEntityModel));
        return collection;
    }

    private FilterDefinition<TGenericEntityModel> GenericFilter(string propertyName, object value)
    {
        var parameter = Expression.Parameter(typeof(TGenericEntityModel), "p");
        var property = Expression.Property(parameter, propertyName);
        var constant = Expression.Constant(value);
        var equality = Expression.Equal(property, constant);
        var lambda = Expression.Lambda<Func<TGenericEntityModel, bool>>(equality, parameter);
        var filter = Builders<TGenericEntityModel>.Filter.Where(lambda);
        return filter;
    }

    private UpdateDefinition<TGenericEntityModel> GenericUpdateFilter(string propertyName, object value)
    {
        var parameter = Expression.Parameter(typeof(TGenericEntityModel), "p");
        var property = Expression.Property(parameter, propertyName);
        var constant = Expression.Constant(value);
        var equality = Expression.Equal(property, constant);
        var lambda = Expression.Lambda<Func<TGenericEntityModel, bool>>(equality, parameter);
        var filter = Builders<TGenericEntityModel>.Update.Set(propertyName, value);
        return filter;
    }

    public async Task CreateItem(TGenericEntityModel entity)
    {
        IMongoCollection<TGenericEntityModel> mongoCollection = CreateMongoDbConnection();
        await mongoCollection.InsertOneAsync(entity);
    }

    public async Task UpdateItem(string propertyNameToFind, object valueToFind, string propertyNameToChange, object valueToChange)
    {
        IMongoCollection<TGenericEntityModel> mongoCollection = CreateMongoDbConnection();
        var filter = GenericFilter(propertyNameToFind, valueToFind);
        var existRecord = await mongoCollection.Find(filter).AnyAsync();
        if (existRecord)
        {
            var update = GenericUpdateFilter(propertyNameToChange, valueToChange);
            await mongoCollection.UpdateOneAsync(filter, update);
        }
    }

    public async Task<bool> ResearchItem(string propertyNameToFind, object valueToFind)
    {
        IMongoCollection<TGenericEntityModel> mongoCollection = CreateMongoDbConnection();
        var filter = GenericFilter(propertyNameToFind, valueToFind);
        var existRecord = await mongoCollection.Find(filter).AnyAsync();
        return existRecord;
    }

    public async Task DeleteItem(string propertyNameToFind, object valueToFind)
    {
        IMongoCollection<TGenericEntityModel> mongoCollection = CreateMongoDbConnection();
        var filter = GenericFilter(propertyNameToFind, valueToFind);
        var existRecord = await mongoCollection.Find(filter).AnyAsync();
        if (existRecord)
        {
            await mongoCollection.DeleteOneAsync(filter);
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
