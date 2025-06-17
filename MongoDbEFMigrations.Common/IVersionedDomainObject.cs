namespace MongoDbEFMigrations.Common;

public interface IVersionedDomainObject
{
    int Version { get; }
}