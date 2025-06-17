using System.ComponentModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbEFMigrations.Common;

namespace ExampleWebService.Domain.Domain.V3;

[DomainVersion(3)]
public class CustomerV3 : IVersionedDomainObject
{
    public int Version => 3;

    [BsonId]
    public ObjectId _id { get; init; }
    
    public required string CustomerId { get; init; }
    public required string FullName { get; init; }
    public required int Age { get; init; }
}