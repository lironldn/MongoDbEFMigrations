using ExampleWebService.Domain.Repo;
using MongoDbEFMigrations.Common;

namespace ExampleWebService.Domain.Domain.V3;

public class ServiceV3(Repository repo, MigrationRunner<IMigrate<CustomerDbEntity>, CustomerDbEntity> migrationRunner)
{
    public async Task AddAsync(CustomerV3 customerDomainLayer)
    {
        var repoLayer = new CustomerDbEntity
        {
            Version = customerDomainLayer.Version,
            CustomerId = customerDomainLayer.CustomerId,
            FullName = customerDomainLayer.FullName,
            Age = customerDomainLayer.Age
        };
        await repo.AddAsync(repoLayer);
    }

    public async Task<CustomerV3?> GetAsync(string id)
    {
        var repoLayer = await repo.GetAsync(id);
        if (repoLayer == null) return null;
        
        var upgraded = migrationRunner.MigrateToVersion<CustomerV3>(repoLayer);
        return upgraded;
    }
}