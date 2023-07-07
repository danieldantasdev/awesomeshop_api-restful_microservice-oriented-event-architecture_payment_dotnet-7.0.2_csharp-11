using System.Linq.Expressions;
using AwesomeShop.Services.Payments.Core.Entities.Interfaces;
using AwesomeShop.Services.Payments.Core.Repositories.Interfaces.Mongo;
using MongoDB.Driver;

namespace AwesomeShop.Services.Payments.Infrastructure.Persistence.Repositories.Implementations.Mongo;

public class MongoRepository<T> : IMongoRepository<T> where T : IEntityBase
{
    private readonly IMongoCollection<T> _mongoCollection;

    public MongoRepository(IMongoDatabase database, string collectionName)
    {
        _mongoCollection = database.GetCollection<T>(collectionName);
    }

    public async Task AddAsync(T entity)
    {
        await _mongoCollection.InsertOneAsync(entity);
    }

    public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _mongoCollection.Find(predicate).ToListAsync();
    }

    public async Task<T> GetAsync(Guid id)
    {
        return await _mongoCollection.Find(e => e.Id.Equals(id)).SingleOrDefaultAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        await _mongoCollection.ReplaceOneAsync(e => e.Id.Equals(entity.Id), entity);
    }
}