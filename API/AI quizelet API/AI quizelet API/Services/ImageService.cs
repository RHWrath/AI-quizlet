using Domain.Entities;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using Infrastructure;

namespace Application.Services;

public class ImageService
{
    private readonly IMongoCollection<Image> _images;

    public ImageService(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _images = database.GetCollection<Image>(settings.Value.ImagesCollection);
    }

    public async Task<List<Image>> GetAllAsync()
        => await _images.Find(_ => true).ToListAsync();
}
