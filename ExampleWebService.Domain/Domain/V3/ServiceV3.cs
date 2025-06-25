using ExampleWebService.Domain.Repo;
using MongoDbEFMigrations.Common;

namespace ExampleWebService.Domain.Domain.V3;

public class ServiceV3(Repository repo, CustomerDbEntityConverter entityVersionConverter)
{
    public async Task AddAsync(CustomerV3 customerDomainLayer)
    {
        var repoLayer = entityVersionConverter.ToDbEntity(customerDomainLayer);
        await repo.AddAsync(repoLayer);
    }

    public async Task<CustomerV3?> GetAsync(string id)
    {
        var repoLayer = await repo.GetAsync(id);
        if (repoLayer == null) return null;
        
        var upgraded = entityVersionConverter.ToDomain<CustomerV3>(repoLayer);
        return upgraded;
    }
}