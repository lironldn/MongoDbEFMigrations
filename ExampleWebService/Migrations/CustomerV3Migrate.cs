using MongoWithEFAndMapper.Repo;

namespace MongoWithEFAndMapper.Migrations;

public class CustomerV3Migrate : IMigrate<CustomerDbEntity>
{
    public int TargetVersion => 3;

    public CustomerDbEntity Upgrade(CustomerDbEntity source)
    {
        if (source.Version.GetValueOrDefault(0) != TargetVersion - 1) return source;
        
        return new CustomerDbEntity
        {
            Version = 3,
            _id = source._id,
            CustomerId = source.CustomerId,
            FullName = source.FullName,
            Age = source.Age ?? 35
        };
    }

    public CustomerDbEntity Downgrade(CustomerDbEntity source)
    {
        throw new NotImplementedException();
    }
}