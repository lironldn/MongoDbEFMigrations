using ExampleWebService.Domain.Repo;
using MongoDbEFMigrations.Common;

namespace ExampleWebService.Domain.Migrations;

public class CustomerV2Migrate : IMigrate<CustomerDbEntity>
{
    public int TargetVersion => 2;

    public CustomerDbEntity Upgrade(CustomerDbEntity source)
    {
        if (source.Version.GetValueOrDefault(0) != TargetVersion - 1) return source;
        
        return new CustomerDbEntity
        {
            Version = 2,
            _id = source._id,
            CustomerId = source.CustomerId,
            FullName = $"This guy: {source.FullName}"
        };
    }

    public CustomerDbEntity Downgrade(CustomerDbEntity source)
    {
        throw new NotImplementedException();
    }
}