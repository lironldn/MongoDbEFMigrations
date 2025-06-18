using System.ComponentModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbEFMigrations.Common;

namespace ExampleWebService.Domain.Domain.V1;

[DomainVersion(1)]
public record CustomerV1
{
    [BsonId]
    public ObjectId _id { get; init; }
    
    public required string CustomerId { get; init; }
    public required string FullName { get; init; }
}