namespace MongoWithEFAndMapper.Repo;

public interface IDbEntity
{
    int? Version { get; set; }
}