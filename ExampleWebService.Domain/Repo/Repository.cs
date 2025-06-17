using Microsoft.EntityFrameworkCore;

namespace ExampleWebService.Domain.Repo;

public class Repository(CustomerDb db)
{
    public async Task AddAsync(CustomerDbEntity customerDbEntity)
    {
        await db.AddAsync(customerDbEntity);
        await db.SaveChangesAsync();
    }

    public async Task<CustomerDbEntity?> GetAsync(string id) =>
        await db.Customers.Where(x => x.CustomerId == id).FirstOrDefaultAsync();
}