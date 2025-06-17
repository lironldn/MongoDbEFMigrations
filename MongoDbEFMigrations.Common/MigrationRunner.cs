using AutoMapper;

namespace MongoDbEFMigrations.Common;

public class MigrationRunner<M,E> where M : IMigrate<E> where E : IDbEntity
{
    private readonly IEnumerable<M> _upgraders;

    public MigrationRunner(IEnumerable<M> upgraders)
    {
        _upgraders = upgraders.OrderBy(x => x.TargetVersion);
    }

    public D MigrateToVersion<D>(E source)
    {
        var targetVersion = DomainVersionAttribute.GetVersion<D>();
        
        E result = source;
        
        if (targetVersion > source.Version)
        {
            // upgrade V0 -> V1 -> V2 etc.
            foreach (var upgrader in _upgraders
                         .Where(u => u.TargetVersion > source.Version.GetValueOrDefault(0) &&
                                     u.TargetVersion <= targetVersion)
                         .OrderBy(u => u.TargetVersion))
            {
                result = upgrader.Upgrade(result);
            }
        }
        else if (targetVersion < source.Version)
        {
            // downgrade V3 -> V2 -> V1 etc.
            foreach (var upgrader in _upgraders
                         .Where(u => u.TargetVersion >= targetVersion &&
                                     u.TargetVersion < source.Version.GetValueOrDefault(0))
                         .OrderByDescending(u => u.TargetVersion))
            {
                result = upgrader.Downgrade(result);
            }
        }

        // this automapping assumes the migrated Repository object has been brought in line
        // with the domain object; if that is not the case, the mapping configuration
        // could be made more bespoke rather than generic as below
        var domain = new MapperConfiguration(cfg =>
            cfg.CreateMap<E, D>())
            .CreateMapper()
            .Map<D>(result);
        
        return domain;
    }
}
