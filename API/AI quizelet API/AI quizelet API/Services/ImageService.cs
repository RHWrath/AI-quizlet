using Entities;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using MongoDbSettings = Services.MongoDbSettings;

namespace Application.Services;

public class ImageService
{
    private readonly IMongoCollection<Image> _images;

    public ImageService(MongoDbSettings settings)
    {
        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);
        _images = database.GetCollection<Image>(settings.ImagesCollection);
    }

    public async Task<List<Image>> GetAllAsync()
        => await _images.Find(_ => true).ToListAsync();

    public async Task<Image?> GetByPostIdAsync(int postId)
        => await _images.Find(i => i.PostID == postId).FirstOrDefaultAsync();
}
