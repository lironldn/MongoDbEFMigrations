using MongoWithEFAndMapper.Repo;

namespace MongoWithEFAndMapper.Migrations;

public interface IMigrate<T> where T : IDbEntity
{
    int TargetVersion { get; }
    T Upgrade(T source);
    T Downgrade(T source);
}