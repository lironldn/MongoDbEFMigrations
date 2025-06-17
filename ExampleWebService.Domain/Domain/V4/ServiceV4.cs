using ExampleWebService.Domain.Repo;
using MongoDbEFMigrations.Common;

namespace ExampleWebService.Domain.Domain.V4;

public class ServiceV4(Repository repo, MigrationRunner<IMigrate<CustomerDbEntity>, CustomerDbEntity> migrationRunner)
{
    public int DomainVersion => 4;
    public async Task AddAsync(CustomerV4 customerDomainLayer)
    {
        var repoLayer = new CustomerDbEntity
        {
            Version = customerDomainLayer.Version,
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
        
        var upgraded = migrationRunner.UpgradeToVersion(repoLayer, DomainVersion);
        return new CustomerV4
        {
            _id = upgraded._id,
            CustomerId = upgraded.CustomerId ?? throw new ArgumentNullException(nameof(upgraded.CustomerId)),
            FullName = upgraded.FullName ?? throw new ArgumentNullException(nameof(upgraded.FullName)),
            DateOfBirth = upgraded.DateOfBirth ?? throw new ArgumentNullException(nameof(upgraded.DateOfBirth)),
        };
    }
}