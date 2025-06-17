using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbEFMigrations.Common;

namespace ExampleWebService.Domain.Repo;

public class CustomerDbEntity : IDbEntity
{
    public int? Version { get; set; }

    [BsonId]
    public ObjectId _id { get; set; }
    public string? CustomerId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FullName { get; set; }
    public int? Age { get; set; }
    public DateTime? DateOfBirth { get; set; }
}