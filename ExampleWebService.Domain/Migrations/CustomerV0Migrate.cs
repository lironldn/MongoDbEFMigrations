using ExampleWebService.Domain.Repo;
using MongoDbEFMigrations.Common;

namespace ExampleWebService.Domain.Migrations;

public class CustomerV0Migrate : IMigrate<CustomerDbEntity>
{
    public int TargetVersion => 0;

    public CustomerDbEntity Upgrade(CustomerDbEntity source)
    {
        throw new NotImplementedException("Nothing to upgrade from");
    }

    public CustomerDbEntity Downgrade(CustomerDbEntity source)
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