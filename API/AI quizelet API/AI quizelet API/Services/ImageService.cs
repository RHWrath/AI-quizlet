using MongoDB.Driver;
using Microsoft.Extensions.Options;
using Services;
using Entities;

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
}
