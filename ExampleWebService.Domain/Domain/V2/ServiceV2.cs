using ExampleWebService.Domain.Repo;
using MongoDbEFMigrations.Common;

namespace ExampleWebService.Domain.Domain.V2;

public class ServiceV2(Repository repo, EntityVersionConverter<CustomerDbEntity> entityVersionConverter)
{
    public async Task AddAsync(CustomerV2 customerDomainLayer)
    {
        var repoLayer = entityVersionConverter.ToDbEntity(customerDomainLayer);
        await repo.AddAsync(repoLayer);
    }

    public async Task<CustomerV2?> GetAsync(string id)
    {
        var repoLayer = await repo.GetAsync(id);
        if (repoLayer == null) return null;
        
        var upgraded = entityVersionConverter.ToDomain<CustomerV2>(repoLayer);
        return upgraded;
    }
}