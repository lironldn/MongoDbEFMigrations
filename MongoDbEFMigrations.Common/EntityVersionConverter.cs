using AutoMapper;

namespace MongoDbEFMigrations.Common;

public class EntityVersionConverter<T> where T : IDbEntity
{
    private readonly IEnumerable<DbEntityMigratorBase<T>> _upgraders;
    private readonly IMapper _mapper;

    public EntityVersionConverter(IEnumerable<DbEntityMigratorBase<T>> upgraders, IMapper mapper)
    {
        _upgraders = upgraders.OrderBy(x => x.TargetVersion);
        _mapper = mapper;
    }

    public D ToDomain<D>(T source)
    {
        var targetVersion = DomainVersionAttribute.GetVersion<D>();

        var result = source;

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

        if (result.Version != targetVersion)
            throw new EntityVersionConverterException($"Failed to migrate to version {targetVersion}. Check all Converters are registered.");

        var domain = _mapper.Map<D>(result);
        return domain;
    }

    public T ToDbEntity<D>(D domainObject)
    {
        var entity = _mapper.Map<T>(domainObject);
        entity.Version = DomainVersionAttribute.GetVersion<D>();
        return entity;
    }
}
