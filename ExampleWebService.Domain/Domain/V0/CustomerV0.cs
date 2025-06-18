using System.ComponentModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbEFMigrations.Common;

namespace ExampleWebService.Domain.Domain.V0;

[DomainVersion(0)]
public record CustomerV0
{
    [BsonId]
    public ObjectId _id { get; init; }
    
    public required string CustomerId { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
}