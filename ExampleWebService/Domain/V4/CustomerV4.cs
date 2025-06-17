using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoWithEFAndMapper.Domain.V4;

public class CustomerV4 : IVersionedDomainObject
{
    public int Version => 4;

    [BsonId]
    public required ObjectId _id { get; init; }
    
    public required string CustomerId { get; init; }
    public required string FullName { get; init; }
    public required DateTime DateOfBirth { get; init; }
}