using ExampleWebService.Domain.Repo;
using MongoDbEFMigrations.Common;

namespace ExampleWebService.Domain.Migrations;

public class CustomerV4Migrate : IMigrate<CustomerDbEntity>
{
    public int TargetVersion => 4;

    public CustomerDbEntity Upgrade(CustomerDbEntity source)
    {
        if (source.Version.GetValueOrDefault(0) != TargetVersion - 1) return source;
        
        return new CustomerDbEntity
        {
            Version = 4,
            _id = source._id,
            CustomerId = source.CustomerId,
            FullName = source.FullName,
            DateOfBirth = source.DateOfBirth ?? DateTime.Today.AddYears(-1 * source.Age.GetValueOrDefault(35))
        };
    }

    public CustomerDbEntity Downgrade(CustomerDbEntity source)
    {
        throw new NotImplementedException();
    }
}