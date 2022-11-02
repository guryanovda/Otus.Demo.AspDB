using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication2.Abstractions
{
    public class BaseEntity
    {
        [BsonId]
        public Guid Id { get; set; }
    }
}
