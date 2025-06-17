using MongoWithEFAndMapper.Repo;

namespace MongoWithEFAndMapper.Migrations;

public class MigrationRunner<M,E> where M : IMigrate<E> where E : IDbEntity
{
    private readonly IEnumerable<M> _upgraders;

    public MigrationRunner(IEnumerable<M> upgraders)
    {
        _upgraders = upgraders.OrderBy(x => x.TargetVersion);
    }

    public E UpgradeToVersion(E source, int targetVersion)
    {
        E result = source;
        foreach (var upgrader in _upgraders
                     .Where(u => u.TargetVersion > source.Version.GetValueOrDefault(0) &&
                                 u.TargetVersion <= targetVersion)
                     .OrderBy(u => u.TargetVersion))
        {
            result = upgrader.Upgrade(result);
        }
        return result;
    }
}
