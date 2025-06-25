using ExampleWebService.Domain.Repo;
using MongoDbEFMigrations.Common;

namespace ExampleWebService.Domain.Domain.V0;

public class ServiceV0(Repository repo, CustomerDbEntityConverter entityVersionConverter)
{
    public async Task AddAsync(CustomerV0 customerDomainLayer)
    {
        var repoLayer = entityVersionConverter.ToDbEntity(customerDomainLayer);
        await repo.AddAsync(repoLayer);
    }

    public async Task<CustomerV0?> GetAsync(string id)
    {
        var repoLayer = await repo.GetAsync(id);
        if (repoLayer == null) return null;
        
        var upgraded = entityVersionConverter.ToDomain<CustomerV0>(repoLayer);
        return upgraded;
    }
}