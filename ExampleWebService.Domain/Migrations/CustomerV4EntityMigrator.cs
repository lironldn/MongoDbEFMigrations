using ExampleWebService.Domain.Repo;
using MongoDbEFMigrations.Common;

namespace ExampleWebService.Domain.Migrations;

public class CustomerV4EntityMigrator : EntityMigratorBase<CustomerDbEntity>
{
    public override int TargetVersion => 4;

    protected override CustomerDbEntity UpgradeEntity(CustomerDbEntity source)
    { 
        return new CustomerDbEntity
        {
            _id = source._id,
            CustomerId = source.CustomerId,
            FullName = source.FullName,
            DateOfBirth = source.DateOfBirth ?? DateTime.Today.AddYears(-1 * source.Age.GetValueOrDefault(35))
        };
    }

    protected override CustomerDbEntity DowngradeEntity(CustomerDbEntity source)
    {
        throw new EntityVersionConverterException("Nothing to downgrade from");
    }
}