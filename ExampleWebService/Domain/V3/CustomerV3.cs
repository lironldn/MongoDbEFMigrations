using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoWithEFAndMapper.Domain.V4;

namespace MongoWithEFAndMapper.Domain.V3;

public class CustomerV3 : IVersionedDomainObject
{
    public int Version => 3;

    [BsonId]
    public required ObjectId _id { get; init; }
    
    public required string CustomerId { get; init; }
    public required string FullName { get; init; }
    public required int Age { get; init; }
}