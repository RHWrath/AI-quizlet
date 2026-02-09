using Entities;
using MongoDB.Driver;
using Microsoft.Extensions.Options;

namespace Services
{
    public class PlayerService
    {
        private readonly IMongoCollection<Player> _players;

        public PlayerService(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _players = database.GetCollection<Player>(settings.Value.PlayersCollection);
        }

        public async Task<List<Player>> GetAllAsync() =>
            await _players.Find(_ => true).ToListAsync();

        public async Task<Player?> GetByIdAsync(string id) =>
            await _players.Find(p => p.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Player player) =>
            await _players.InsertOneAsync(player);

        public async Task UpdateScoreAsync(string id, int newScore) =>
            await _players.UpdateOneAsync(p => p.Id == id,
                Builders<Player>.Update.Set(p => p.Score, newScore));
    }
}
