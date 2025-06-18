using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbEFMigrations.Common;

namespace ExampleWebService.Domain.Domain.V4;

[DomainVersion(4)]
public record CustomerV4
{
    [BsonId]
    public ObjectId _id { get; init; }
    
    public required string CustomerId { get; init; }
    public required string FullName { get; init; }
    public required DateTime Birthday { get; init; }
}