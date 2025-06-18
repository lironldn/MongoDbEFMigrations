using ExampleWebService.Domain.Domain.V3;
using ExampleWebService.Domain.Repo;
using MongoDbEFMigrations.Common;

namespace ExampleWebService.Domain.Migrations;

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
            Age = source.Age ?? CustomerV3.DefaultAge
        };
    }

    public CustomerDbEntity Downgrade(CustomerDbEntity source)
    {
        var age = DateTime.Today.Year - source.DateOfBirth?.Year;
        if (!source.DateOfBirth.HasValue) age = CustomerV3.DefaultAge;
        return new CustomerDbEntity
        {
            Version = TargetVersion,
            _id = source._id,
            CustomerId = source.CustomerId,
            FullName = source.FullName,
            Age = age
        };
    }
}