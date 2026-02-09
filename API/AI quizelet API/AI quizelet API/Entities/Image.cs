using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public class Image
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    // Opslaan als byte array
    public byte[] Data { get; set; } = null!;

    // Eventueel extra metadata
    public string FileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
}
