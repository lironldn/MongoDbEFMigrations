using ExampleWebService.Domain.Repo;
using MongoDbEFMigrations.Common;

namespace ExampleWebService.Domain.Migrations;

public class CustomerV0EntityMigrator : EntityMigratorBase<CustomerDbEntity>
{
    public override int TargetVersion => 0;

    protected override CustomerDbEntity UpgradeEntity(CustomerDbEntity source)
    {
        throw new EntityVersionConverterException("Nothing to upgrade from");
    }

    protected override CustomerDbEntity DowngradeEntity(CustomerDbEntity source)
    {
        var name = source.FullName?.Split(' ');
        return new CustomerDbEntity
        {
            CustomerId = source.CustomerId,
            FirstName = name?[0] ?? null,
            LastName = name?[1] ?? null
        };
    }
}