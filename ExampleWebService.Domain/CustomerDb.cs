using ExampleWebService.Domain.Repo;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace ExampleWebService.Domain;

public class CustomerDb(DbContextOptions<CustomerDb> options) : DbContext(options)
{
    public DbSet<CustomerDbEntity> Customers { get; init; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CustomerDbEntity>().ToCollection("customers_ef");
        base.OnModelCreating(modelBuilder);
    }
}
