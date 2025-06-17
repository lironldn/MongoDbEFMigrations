namespace MongoDbEFMigrations.Common;

public interface IDbEntity
{
    int? Version { get; set; }
}