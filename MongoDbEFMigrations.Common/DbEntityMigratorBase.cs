namespace MongoDbEFMigrations.Common;

public abstract class DbEntityMigratorBase<T> where T : IDbEntity
{
    public abstract int TargetVersion { get; }
    protected abstract T UpgradeEntity(T source);
    protected abstract T DowngradeEntity(T source);

    public T Upgrade(T source)
    {
        if (TargetVersion != source.Version.GetValueOrDefault(0) + 1)
            throw new EntityVersionConverterException(
                $"Cannot upgrade from version {source.Version} to Target version {TargetVersion}. Check all Converters are registered.");
        
        var entity = UpgradeEntity(source);
        entity.Version = TargetVersion;
        return entity;
    }

    public T Downgrade(T source)
    {
        if (TargetVersion != source.Version.GetValueOrDefault(0) - 1)
            throw new EntityVersionConverterException(
                $"Cannot downgrade from version {source.Version} to Target version {TargetVersion}. Check all Converters are registered.");
        
        var entity = DowngradeEntity(source);
        entity.Version = TargetVersion;
        return entity;
    }
}