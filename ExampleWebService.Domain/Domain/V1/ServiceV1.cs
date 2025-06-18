using ExampleWebService.Domain.Repo;
using MongoDbEFMigrations.Common;

namespace ExampleWebService.Domain.Domain.V1;

public class ServiceV1(Repository repo, MigrationRunner<CustomerDbEntity> migrationRunner)
{
    public async Task AddAsync(CustomerV1 customerDomainLayer)
    {
        var repoLayer = new CustomerDbEntity
        {
            Version = DomainVersionAttribute.GetVersion<CustomerV1>(),
            CustomerId = customerDomainLayer.CustomerId,
            FullName = customerDomainLayer.FullName
        };
        await repo.AddAsync(repoLayer);
    }

    public async Task<CustomerV1?> GetAsync(string id)
    {
        var repoLayer = await repo.GetAsync(id);
        if (repoLayer == null) return null;
        
        var upgraded = migrationRunner.MigrateToVersion<CustomerV1>(repoLayer);
        return upgraded;
    }
}