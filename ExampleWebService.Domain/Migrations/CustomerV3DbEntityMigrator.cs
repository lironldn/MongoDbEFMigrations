using ExampleWebService.Domain.Domain.V3;
using ExampleWebService.Domain.Repo;
using MongoDbEFMigrations.Common;

namespace ExampleWebService.Domain.Migrations;

public class CustomerV3DbEntityMigrator : DbEntityMigratorBase<CustomerDbEntity>
{
    public override int TargetVersion => 3;

    protected override CustomerDbEntity UpgradeEntity(CustomerDbEntity source)
    {
        return new CustomerDbEntity
        {
            _id = source._id,
            CustomerId = source.CustomerId,
            FullName = source.FullName,
            Age = source.Age ?? CustomerV3.DefaultAge
        };
    }

    protected override CustomerDbEntity DowngradeEntity(CustomerDbEntity source)
    {
        var age = DateTime.Today.Year - source.DateOfBirth?.Year;
        if (!source.DateOfBirth.HasValue) age = CustomerV3.DefaultAge;
        return new CustomerDbEntity
        {
            _id = source._id,
            CustomerId = source.CustomerId,
            FullName = source.FullName,
            Age = age
        };
    }
}