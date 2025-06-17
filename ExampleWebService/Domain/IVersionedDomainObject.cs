namespace MongoWithEFAndMapper.Domain;

public interface IVersionedDomainObject
{
    int Version { get; }
}