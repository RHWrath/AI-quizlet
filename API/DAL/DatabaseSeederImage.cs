using MongoDB.Driver;
using System.Text.Json;
using AIQuizlet.Models;

namespace AIQuizlet.Seeding
{
    public class ImageSeeder
    {
        private readonly IMongoCollection<ImageModel> _images;
        private readonly ILogger<ImageSeeder> _logger;
        private readonly string _seedFilePath;

        public ImageSeeder(IConfiguration configuration, ILogger<ImageSeeder> logger)
        {
            _logger = logger;

            // Connect to MongoDB
            var connectionString = configuration.GetConnectionString("MongoDB")
                ?? "mongodb://localhost:27017/quizlet";

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("quizlet");
            _images = database.GetCollection<ImageModel>("images");

            // Path to the seed JSON file
            _seedFilePath = configuration["SeedFilePath"]
                ?? Path.Combine(Directory.GetCurrentDirectory(), "DAL", "mongodb-configs", "images_seed.json");
        }

        public async Task SeedAsync(bool clearExisting = false)
        {
            // Check if collection already has data
            var count = await _images.CountDocumentsAsync(_ => true);

            if (count > 0 && !clearExisting)
            {
                _logger.LogInformation("Images collection already has {Count} documents. Skipping seed. Use clearExisting=true to reseed.", count);
                return;
            }

            // Clear existing data if requested
            if (clearExisting && count > 0)
            {
                await _images.DeleteManyAsync(_ => true);
                _logger.LogInformation("Cleared {Count} existing image documents.", count);
            }

            // Read the seed JSON file
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

            // Map seed data to ImageModel
            var images = seedData.Select(item => new ImageModel
            {
                Link = item.Link,
                AI = item.AI
            }).ToList();

            // Insert into MongoDB
            await _images.InsertManyAsync(images);
            _logger.LogInformation("Successfully seeded {Count} images into the database.", images.Count);

            // Log a summary
            var aiCount = images.Count(x => x.AI);
            var realCount = images.Count(x => !x.AI);
            _logger.LogInformation("Seed summary: {AI} AI images, {Real} real images.", aiCount, realCount);
        }

        // DTO for deserializing the seed JSON
        private class ImageSeedItem
        {
            public int PostId { get; set; }
            public string Link { get; set; } = string.Empty;
            public bool AI { get; set; }
        }
    }
}
