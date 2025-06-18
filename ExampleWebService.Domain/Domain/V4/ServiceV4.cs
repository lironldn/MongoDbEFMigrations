using ExampleWebService.Domain.Repo;
using MongoDbEFMigrations.Common;

namespace ExampleWebService.Domain.Domain.V4;

public class ServiceV4(Repository repo, MigrationRunner<CustomerDbEntity> migrationRunner)
{
    public async Task AddAsync(CustomerV4 customerDomainLayer)
    {
        var repoLayer = new CustomerDbEntity
        {
            Version = DomainVersionAttribute.GetVersion<CustomerV4>(),
            CustomerId = customerDomainLayer.CustomerId,
            FullName = customerDomainLayer.FullName,
            DateOfBirth = customerDomainLayer.DateOfBirth
        };
        await repo.AddAsync(repoLayer);
    }

    public async Task<CustomerV4?> GetAsync(string id)
    {
        var repoLayer = await repo.GetAsync(id);
        if (repoLayer == null) return null;
        
        var upgraded = migrationRunner.MigrateToVersion<CustomerV4>(repoLayer);
        return upgraded;
    }
}