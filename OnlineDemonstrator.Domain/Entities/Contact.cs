using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OnlineDemonstrator.Libraries.Domain.Entities
{
    public class Contact
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Author { get; set; }

        public string Email { get; set; }

        public string Github { get; set; }
    }
}