using ExampleWebService.Domain.Repo;
using MongoDbEFMigrations.Common;

namespace ExampleWebService.Domain.Domain.V0;

public class ServiceV0(Repository repo, MigrationRunner<CustomerDbEntity> migrationRunner)
{
    public async Task AddAsync(CustomerV0 customerDomainLayer)
    {
        var repoLayer = new CustomerDbEntity
        {
            Version = DomainVersionAttribute.GetVersion<CustomerV0>(),
            CustomerId = customerDomainLayer.CustomerId,
            FirstName = customerDomainLayer.FirstName,
            LastName = customerDomainLayer.LastName,
        };
        await repo.AddAsync(repoLayer);
    }

    public async Task<CustomerV0?> GetAsync(string id)
    {
        var repoLayer = await repo.GetAsync(id);
        if (repoLayer == null) return null;
        
        var upgraded = migrationRunner.MigrateToVersion<CustomerV0>(repoLayer);
        return upgraded;
    }
}