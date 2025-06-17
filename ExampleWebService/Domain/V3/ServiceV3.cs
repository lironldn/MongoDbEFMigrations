using MongoWithEFAndMapper.Migrations;
using MongoWithEFAndMapper.Repo;

namespace MongoWithEFAndMapper.Domain.V3;

public class ServiceV3(Repository repo, MigrationRunner<IMigrate<CustomerDbEntity>, CustomerDbEntity> migrationRunner)
{
    public int DomainVersion => 3;
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
        
        var upgraded = migrationRunner.UpgradeToVersion(repoLayer, DomainVersion);
        return new CustomerV3
        {
            _id = upgraded._id,
            CustomerId = upgraded.CustomerId ?? throw new ArgumentNullException(nameof(upgraded.CustomerId)),
            FullName = upgraded.FullName ?? throw new ArgumentNullException(nameof(upgraded.FullName)),
            Age = upgraded.Age ?? throw new ArgumentNullException(nameof(upgraded.Age)),
        };
    }
}