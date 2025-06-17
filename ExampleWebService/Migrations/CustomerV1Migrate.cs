using MongoWithEFAndMapper.Repo;

namespace MongoWithEFAndMapper.Migrations;

public class CustomerV1Migrate : IMigrate<CustomerDbEntity>
{
    public int TargetVersion => 1;

    public CustomerDbEntity Upgrade(CustomerDbEntity source)
    {
        if (source.Version.GetValueOrDefault(0) != TargetVersion - 1) return source;
        
        return new CustomerDbEntity
        {
            Version = 1,
            _id = source._id,
            CustomerId = source.CustomerId,
            FullName = $"{source.FirstName} {source.LastName}"
        };
    }

    public CustomerDbEntity Downgrade(CustomerDbEntity source)
    {
        throw new NotImplementedException();
    }
}