using ExampleWebService.Domain.Domain.V4;
using ExampleWebService.Domain.Migrations;
using ExampleWebService.Domain.Repo;
using FluentAssertions;

namespace MongoDbEFMigrations.Common.UnitTests;

public class MigrationRunnerTests
{
    private MigrationRunner<CustomerDbEntity> _runner;

    [SetUp]
    public void Setup()
    {
        _runner = new MigrationRunner<CustomerDbEntity>(
            new List<IMigrate<CustomerDbEntity>>
            {
                new CustomerV1Migrate(),
                new CustomerV2Migrate(),
                new CustomerV3Migrate(),
                new CustomerV4Migrate()
            }
        );
    }

    [Test]
    public void UpgradeV3_to_V4()
    {
        var entity = new CustomerDbEntity
        {
            Version = 3,
            CustomerId = "c1",
            FullName = "John Doe",
            Age = 40
        };
        
        var result = _runner.MigrateToVersion<CustomerV4>(entity);
        result.CustomerId.Should().Be("c1");
        result.FullName.Should().Be("John Doe");
        result.DateOfBirth.Should().Be(DateTime.Today.AddYears(-40));
    }
}