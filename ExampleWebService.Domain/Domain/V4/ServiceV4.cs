using ExampleWebService.Domain.Repo;
using MongoDbEFMigrations.Common;

namespace ExampleWebService.Domain.Domain.V4;

public class ServiceV4(Repository repo, EntityVersionConverter<CustomerDbEntity> entityVersionConverter)
{
    public async Task AddAsync(CustomerV4 customerDomainLayer)
    {
        var repoLayer = entityVersionConverter.ToDbEntity(customerDomainLayer);
        await repo.AddAsync(repoLayer);
    }

    public async Task<CustomerV4?> GetAsync(string id)
    {
        var repoLayer = await repo.GetAsync(id);
        if (repoLayer == null) return null;
        
        var upgraded = entityVersionConverter.ToDomain<CustomerV4>(repoLayer);
        return upgraded;
    }
}