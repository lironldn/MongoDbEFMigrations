using AutoMapper;
using ExampleWebService.Domain.Migrations;
using ExampleWebService.Domain.Repo;
using MongoDbEFMigrations.Common;

namespace ExampleWebService.Domain.Domain;

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