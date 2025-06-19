using ExampleWebService.Domain.Repo;
using MongoDbEFMigrations.Common;

namespace ExampleWebService.Domain.Migrations;

public class CustomerV2EntityMigrator : EntityMigratorBase<CustomerDbEntity>
{
    public override int TargetVersion => 2;

    protected override CustomerDbEntity UpgradeEntity(CustomerDbEntity source)
    {
        return new CustomerDbEntity
        {
            _id = source._id,
            CustomerId = source.CustomerId,
            FullName = $"This one -> {source.FullName}"
        };
    }

    protected override CustomerDbEntity DowngradeEntity(CustomerDbEntity source)
    {
        return new CustomerDbEntity
        {
            _id = source._id,
            CustomerId = source.CustomerId,
            FullName = source.FullName
        };
    }
}