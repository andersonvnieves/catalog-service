using br.com.fiap.cloudgames.Catalog.Domain.Aggregates;
using br.com.fiap.cloudgames.Catalog.Domain.Repositories;
using MongoDB.Driver;

namespace br.com.fiap.cloudgames.Catalog.Infrastructure.Persistence.MongoDB.Repositories
{
    public class LibraryRepository : ILibraryRepository
    {
        private readonly IMongoCollection<Library> _librariesCollection;

        public LibraryRepository(IMongoDatabase database)
        {
            _librariesCollection = database.GetCollection<Library>("libraries");
        }
                
        public async Task AddAsync(Library library)
        {
            await _librariesCollection.InsertOneAsync(library);
        }

        public async Task UpdateAsync(Library library)
        {
            var filter = Builders<Library>.Filter.Eq(l => l.UserId, library.UserId);
            await _librariesCollection.ReplaceOneAsync(filter, library, new ReplaceOptions { IsUpsert = true });
        }

        public async Task<Library?> GetByUserIdAsync(Guid userId)
        {
            var filter = Builders<Library>.Filter.Eq(l => l.UserId, userId);
            return await _librariesCollection.Find(filter).FirstOrDefaultAsync();
        }
    }
}
