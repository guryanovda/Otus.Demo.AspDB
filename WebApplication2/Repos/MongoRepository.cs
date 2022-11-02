using MongoDB.Driver;
using SharpCompress.Common;
using WebApplication2.Abstractions;

namespace WebApplication2.Repos
{
    public class MongoRepository<T>: IRepository<T> where T : BaseEntity
    {
        private readonly IMongoCollection<T> _collection;
        public MongoRepository(MongoUrl mongoUrl)
        {
            var dbClient = new MongoClient(mongoUrl);
            var db = dbClient.GetDatabase(mongoUrl.DatabaseName);
            _collection = db.GetCollection<T>($"{typeof(T).Name.ToLower()}s");
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<T?> GetAsync(Guid id)
        {
            return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            await _collection.ReplaceOneAsync(x=> x.Id == entity.Id, entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _collection.DeleteOneAsync(x => x.Id == id);
        }
    }
}
