using ExampleWebService.Domain.Repo;
using MongoDbEFMigrations.Common;

namespace ExampleWebService.Domain.Domain.V2;

public class ServiceV2(Repository repo, MigrationRunner<CustomerDbEntity> migrationRunner)
{
    public async Task AddAsync(CustomerV2 customerDomainLayer)
    {
        var repoLayer = new CustomerDbEntity
        {
            Version = DomainVersionAttribute.GetVersion<CustomerV2>(),
            CustomerId = customerDomainLayer.CustomerId,
            FullName = customerDomainLayer.FullName
        };
        await repo.AddAsync(repoLayer);
    }

    public async Task<CustomerV2?> GetAsync(string id)
    {
        var repoLayer = await repo.GetAsync(id);
        if (repoLayer == null) return null;
        
        var upgraded = migrationRunner.MigrateToVersion<CustomerV2>(repoLayer);
        return upgraded;
    }
}