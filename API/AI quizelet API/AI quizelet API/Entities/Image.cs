using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Entities
{
    public class Image
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _Id { get; set; } = null!;

        public string link { get; set; } = null!;   // pad naar bestand
        public bool AI { get; set; }
        public int postId { get; set; }
    }
}
