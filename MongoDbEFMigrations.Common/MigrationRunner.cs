using AutoMapper;

namespace MongoDbEFMigrations.Common;

public class MigrationRunner<T> where T : IDbEntity
{
    private readonly IEnumerable<IMigrate<T>> _upgraders;
    private readonly IMapper _mapper;

    public MigrationRunner(IEnumerable<IMigrate<T>> upgraders, IMapper mapper)
    {
        _upgraders = upgraders.OrderBy(x => x.TargetVersion);
        _mapper = mapper;
    }

    public D MigrateToVersion<D>(T source)
    {
        var targetVersion = DomainVersionAttribute.GetVersion<D>();
        
        T result = source;
        
        if (targetVersion > source.Version.GetValueOrDefault(0))
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
        else if (targetVersion < source.Version.GetValueOrDefault(0))
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
        
        var domain = _mapper.Map<D>(result);
        return domain;
    }
}
