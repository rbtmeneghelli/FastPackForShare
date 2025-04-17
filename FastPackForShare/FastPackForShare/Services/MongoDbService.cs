using System.Linq.Expressions;
using FastPackForShare.Default;
using FastPackForShare.Interfaces;
using FastPackForShare.Services.Bases;
using MongoDB.Driver;

namespace FastPackForShare.Services;

public sealed class MongoDbService<TBaseEntityModel> : BaseHandlerService, IMongoDbService<TBaseEntityModel> where TBaseEntityModel : BaseEntityModel
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

    private IMongoCollection<TBaseEntityModel> CreateMongoDbConnection()
    {
        var client = new MongoClient(_connectionString);
        var database = client.GetDatabase("testdb");
        var collection = database.GetCollection<TBaseEntityModel>(nameof(TBaseEntityModel));
        return collection;
    }

    private FilterDefinition<TBaseEntityModel> GenericFilter(string propertyName, object value)
    {
        var parameter = Expression.Parameter(typeof(TBaseEntityModel), "p");
        var property = Expression.Property(parameter, propertyName);
        var constant = Expression.Constant(value);
        var equality = Expression.Equal(property, constant);
        var lambda = Expression.Lambda<Func<TBaseEntityModel, bool>>(equality, parameter);
        var filter = Builders<TBaseEntityModel>.Filter.Where(lambda);
        return filter;
    }

    private UpdateDefinition<TBaseEntityModel> GenericUpdateFilter(string propertyName, object value)
    {
        var parameter = Expression.Parameter(typeof(TBaseEntityModel), "p");
        var property = Expression.Property(parameter, propertyName);
        var constant = Expression.Constant(value);
        var equality = Expression.Equal(property, constant);
        var lambda = Expression.Lambda<Func<TBaseEntityModel, bool>>(equality, parameter);
        var filter = Builders<TBaseEntityModel>.Update.Set(propertyName, value);
        return filter;
    }

    public async Task CreateItem(TBaseEntityModel entity)
    {
        IMongoCollection<TBaseEntityModel> mongoCollection = CreateMongoDbConnection();
        await mongoCollection.InsertOneAsync(entity);
    }

    public async Task UpdateItem(string propertyNameToFind, object valueToFind, string propertyNameToChange, object valueToChange)
    {
        IMongoCollection<TBaseEntityModel> mongoCollection = CreateMongoDbConnection();
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
        IMongoCollection<TBaseEntityModel> mongoCollection = CreateMongoDbConnection();
        var filter = GenericFilter(propertyNameToFind, valueToFind);
        var existRecord = await mongoCollection.Find(filter).AnyAsync();
        return existRecord;
    }

    public async Task DeleteItem(string propertyNameToFind, object valueToFind)
    {
        IMongoCollection<TBaseEntityModel> mongoCollection = CreateMongoDbConnection();
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
