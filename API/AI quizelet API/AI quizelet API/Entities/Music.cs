using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Entities
{
    public class Music
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _Id { get; set; } = null!;

        public string link { get; set; } = null!;
        public bool AI { get; set; }
        public int postId { get; set; }
    }
}
