using MongoDB.Driver;
using MongoDB.Bson;
using System.Text.Json;

namespace AIQuizlet.Seeding
{
    public class ImageSeeder
    {
        private readonly IMongoCollection<BsonDocument> _images;
        private readonly ILogger<ImageSeeder> _logger;
        private readonly string _seedFilePath;

        public ImageSeeder(IConfiguration configuration, ILogger<ImageSeeder> logger)
        {
            _logger = logger;

            
            var connectionString = configuration.GetConnectionString("MongoDB")
                ?? "mongodb://localhost:27017/quizlet";

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("quizlet");

            
            _images = database.GetCollection<BsonDocument>("images");

            
            _seedFilePath = configuration["SeedFilePath"]
                ?? Path.Combine(Directory.GetCurrentDirectory(), "DAL", "mongodb-configs", "images_seed.json");
        }

        
        public async Task SeedAsync(bool clearExisting = false)
        {
            
            var count = await _images.CountDocumentsAsync(_ => true);

            if (count > 0 && !clearExisting)
            {
                _logger.LogInformation("Images collection already has {Count} documents. Skipping seed.", count);
                return;
            }

            
            if (clearExisting && count > 0)
            {
                await _images.DeleteManyAsync(_ => true);
                _logger.LogInformation("Cleared {Count} existing image documents.", count);
            }

            
            if (!File.Exists(_seedFilePath))
            {
                _logger.LogError("Seed file not found at: {Path}", _seedFilePath);
                return;
            }

            var jsonContent = await File.ReadAllTextAsync(_seedFilePath);
            var seedData = JsonSerializer.Deserialize<List<ImageSeedItem>>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (seedData == null || seedData.Count == 0)
            {
                _logger.LogWarning("Seed file is empty or invalid.");
                return;
            }

            
            var documents = seedData.Select(item => new BsonDocument
            {
                { "postId", item.PostId },
                { "link", item.Link },
                { "AI", item.AI }
            }).ToList();

            
            await _images.InsertManyAsync(documents);
            _logger.LogInformation("Successfully seeded {Count} images.", documents.Count);

            
            var aiCount = seedData.Count(x => x.AI);
            var realCount = seedData.Count(x => !x.AI);
            _logger.LogInformation("Seed summary: {AI} AI images, {Real} real images.", aiCount, realCount);
        }

        
        private class ImageSeedItem
        {
            public int PostId { get; set; }
            public string Link { get; set; } = string.Empty;
            public bool AI { get; set; }
        }
    }
}