using ExampleWebService.Domain.Repo;
using MongoDbEFMigrations.Common;

namespace ExampleWebService.Domain.Migrations;

public class CustomerV1EntityMigrator : EntityMigratorBase<CustomerDbEntity>
{
    public override int TargetVersion => 1;

    protected override CustomerDbEntity UpgradeEntity(CustomerDbEntity source)
    {
        return new CustomerDbEntity
        {
            _id = source._id,
            CustomerId = source.CustomerId,
            FullName = $"{source.FirstName} {source.LastName}"
        };
    }

    protected override CustomerDbEntity DowngradeEntity(CustomerDbEntity source)
    {
        return new CustomerDbEntity
        {
            _id = source._id,
            CustomerId = source.CustomerId,
            FullName = source.FullName?.Replace("This one -> ", "") ?? null
        };
    }
}