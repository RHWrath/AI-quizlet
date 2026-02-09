using Entities;
using MongoDB.Driver;
using Microsoft.Extensions.Options;

namespace Services
{
    public class MusicService
    {
        private readonly IMongoCollection<Music> _music;

        public MusicService(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _music = database.GetCollection<Music>(settings.Value.MusicCollection);
        }

        public async Task<List<Music>> GetAllAsync() =>
            await _music.Find(_ => true).ToListAsync();
    }
}
