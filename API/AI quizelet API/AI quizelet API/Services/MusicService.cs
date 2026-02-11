using Entities;
using MongoDB.Driver;
using Microsoft.Extensions.Options;

namespace Services
{
    public class MusicService
    {
        private readonly IMongoCollection<Music> _music;

        public MusicService(MongoDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _music = database.GetCollection<Music>(settings.MusicCollection);
        }

        public async Task<List<Music>> GetAllAsync() =>
            await _music.Find(_ => true).ToListAsync();

        public async Task<Music?> GetByPostIdAsync(int postId) =>
            await _music.Find(m => m.postId == postId).FirstOrDefaultAsync();
    }
}
