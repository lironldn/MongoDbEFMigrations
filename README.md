# MongoDbEFMigrations

## What is it?
One problem when using Entity Framework (EF) with MongoDB is that of data migrations. This includes breaking changes where:
- A new mandatory field is added to a model
- A field is renamed
- A field type is changed

MongoDB is schema-free and so writes to the database will work just fine. However, when trying to fetch these entities from the database,
EF will break when trying to deserialise the documents into the strongly typed objects.

## Database data migrations
The problem with migrating the data in one sweep is that the service will be broken during this time, resulting in downtime.
Instead or in addition to this, a softer, 'On-The-Fly' approach means that the data can be upgraded on-demand, until it is eventually upgraded.

## On-The-Fly migrations
This approach means that when we fetch a record from the database that is different to the version we want, we can migrate it on-the-fly.
This will support both upgrades, but also downgrades for when an older version of the API needs to still be supported.
---
## Library structure

The library includes:
- An `IDbEntity` interface that the database entities must implement,
  which enforces versioned database entities.
- A `DomainVersionAttribute` to decorate the versioned domain entities.
  This will allow the converter to know what version to target.
- An abstract `DbEntityMigratorBase`, which does in-place migrations
  of the database entities before they are mapped to the target
  domain object.
- An abstract `EntityVersionCoverter` which serves two purpose:
  - Migrate a database entity to the target domain object;
  - Convert a domain object to a database entity.
- An example implementation and unit tests for `CustomerDbEntity` and `CustomerVx`
domain objects.

## Usage

See the `Libraries.Shared.EntityConversion.UnitTests` example for usage.

### Step 1: Define your database entity
Your DbEntity must:
- Implement `IDbEntity`
- Have **only optional** fields
- "One size fits all": as the version evolve, this entity must remain compatible
  with all versions of the data - if a field name is changed,
  the DbEntity must contain fields for both the old and new name;
  if a field type is modified, this will require renaming the field too,
  in order to support both version side-by-side.

Example DbEntity:
```
public record CustomerDbEntity : IDbEntity
{
    public int? Version { get; set; }

    public string? CustomerId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FullName { get; set; }
    public int? Age { get; set; }
    public DateTime? DateOfBirth { get; set; }
}
```
Note that the entity has fields of V0, V1 all the way to V4,
as Entity Framework will use the same entity to read all versions
in the database - "one size fits all".

### Step 2: Define your domain objects
Your Domain objects must:
- Be decorated with `[DomainVersion(x)]` - x will be the version;
- Can contain both mandatory and optional fields

Example (see more in the unit tests project):
```
[DomainVersion(3)]
public record CustomerV3
{
    public const int DefaultAge = 35;

    public required string CustomerId { get; init; }
    public required string FullName { get; init; }
    public required int Age { get; init; }
}
```

### Step 3: Write your Migrators
For each supported version of the domain, a migrator
needs to be written. Implement the `DbEntityMigratorBase`
class for each version, it has an _UpgradeEntity_ and _DowngradeEntity_
abstract methods.

Example:
```
public class CustomerV3DbEntityMigrator : DbEntityMigratorBase<CustomerDbEntity>
{
    public override int TargetVersion => 3;

    protected override CustomerDbEntity UpgradeEntity(CustomerDbEntity source)
    {
        return new CustomerDbEntity
        {
            CustomerId = source.CustomerId,
            FullName = source.FullName,
            // Age is introduced in V3 - set a default age for V2 or older records
            Age = source.Age ?? CustomerV3.DefaultAge
        };
    }

    protected override CustomerDbEntity DowngradeEntity(CustomerDbEntity source)
    {
        // downgrade from V4 - calculate the age from the DateOfBirth
        // (V4 introduces a DateOfBirth to replace Age)
        var age = DateTime.Today.Year - source.DateOfBirth?.Year;
        if (!source.DateOfBirth.HasValue) age = CustomerV3.DefaultAge;
        return new CustomerDbEntity
        {
            CustomerId = source.CustomerId,
            FullName = source.FullName,
            Age = age
        };
    }
}
```
Note that the Age field is mandatory, but to support older
versions, a default value is set when reading those values -
it is up to you what the default behaviour should be.

### Step 4: Implement a DbEntityConverter
The converter, one for each type of entity, takes in:
- A list of Migrators, one for each target version
- An `IMapper`, using [AutoMapper](https://automapper.org) to
  map between a DbEntity and a domain object.

Example IMapper configuration:
```
public static class AutoMapperConfig
{
    public static IMapper CreateMapper()
    {
        var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CustomerDbEntity, CustomerV0>();
                cfg.CreateMap<CustomerDbEntity, CustomerV1>();
                cfg.CreateMap<CustomerDbEntity, CustomerV2>();
                cfg.CreateMap<CustomerDbEntity, CustomerV3>();
                cfg.CreateMap<CustomerDbEntity, CustomerV4>()
                    .ForMember(dest => dest.Birthday, opt
                        => opt.MapFrom(src => src.DateOfBirth));

                cfg.CreateMap<CustomerV0, CustomerDbEntity>();
                cfg.CreateMap<CustomerV1, CustomerDbEntity>();
                cfg.CreateMap<CustomerV2, CustomerDbEntity>();
                cfg.CreateMap<CustomerV3, CustomerDbEntity>();
                cfg.CreateMap<CustomerV4, CustomerDbEntity>()
                    .ForMember(dest => dest.DateOfBirth, opt
                        => opt.MapFrom(src => src.Birthday));
            });

        return mapperConfig.CreateMapper();
    }
}
```
In this example, each version of the domain has to be mapped.
In some cases (in this example DateOfBirth -> Birthday), fields in the DbEntity can be mapped to a different
field name in the domain, supporting renaming of fields without
requiring database migration.

The Converter is then defined as follows:

```
public class CustomerDbEntityConverter(IEnumerable<DbEntityMigratorBase<CustomerDbEntity>> migrators, IMapper mapper)
    : EntityVersionConverter<CustomerDbEntity>
    (
        migrators,
        mapper
    )
{
    public CustomerDbEntityConverter() :
        this([
            new CustomerV0DbEntityMigrator(),
            new CustomerV1DbEntityMigrator(),
            new CustomerV2DbEntityMigrator(),
            new CustomerV3DbEntityMigrator(),
            new CustomerV4DbEntityMigrator(),
        ],
            AutoMapperConfig.CreateMapper()
            )
    {
    }
}
```

### Step 5: Register the Converter in your DI
```
        builder.Services.AddSingleton<CustomerDbEntityConverter>()
```

### Step 6: Inject and use your converter
Inject the converter into your service and use it as follows:

```
public class ServiceV2(Repository repo, CustomerDbEntityConverter entityVersionConverter)
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
```
Note the usage of:
- `ToDbEntity(domainObject)` on write
- `ToDomain<CustomerV2>(dbEntity)` on read.

Both of these methods rely on the mapping to be registered correctly
in both directions: from DbEntity to Domain and from Domain to DbEntity.

---
# Running the example implementation
This code also contains a concrete example implementation.
Clone the repo and run it against a local MongoDB - you may need to change the application settings.

Once running, you can hit different versions of the service to see how different versions of the data are returned.

```
POST http://localhost:5184/customer/v0

{
    "CustomerId": "c0",
    "FirstName": "John",
    "LastName": "Doe"
}
```

```
POST http://localhost:5184/customer/v3

{
    "CustomerId": "c3",
    "FullName": "John Doe",
    "Age": 30
}
```

```
POST http://localhost:5184/customer/v4

{
    "CustomerId": "c4",
    "FullName": "John Doe",
    "Birthday": "1978-12-01"
}
```
In the last V4 example, if you look at the database, the field `Birthday` will be stored as `DateOfBirth`.

Then try fetching the two items and see how they are upgraded.
```
GET http://localhost:5184/customer/v4/c0 - upgrade V0 -> V1 -> V2 -> V3 -> V4
```
```
GET http://localhost:5184/customer/v3/c3 - no upgrade
GET http://localhost:5184/customer/v4/c3 - upgraded (DateOfBirth calculated from Age)
```
```
GET http://localhost:5184/customer/v0/c4 - downgrade V4 -> V3 -> V2 -> V1 -> V0
GET http://localhost:5184/customer/v3/c4 - downgrade V4 -> V3
GET http://localhost:5184/customer/v4/c4 - no upgrade (but 'DateOfBirth' is mapped to 'Birthday')
```
