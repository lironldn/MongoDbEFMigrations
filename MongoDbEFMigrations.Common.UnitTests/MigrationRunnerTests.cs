using ExampleWebService.Domain.Domain;
using ExampleWebService.Domain.Domain.V0;
using ExampleWebService.Domain.Domain.V1;
using ExampleWebService.Domain.Domain.V2;
using ExampleWebService.Domain.Domain.V3;
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
                new CustomerV0Migrate(),
                new CustomerV1Migrate(),
                new CustomerV2Migrate(),
                new CustomerV3Migrate(),
                new CustomerV4Migrate()
            },
            AutoMapperConfig.CreateMapper()
        );
    }

    [Test]
    public void UpgradeV0_to_V1()
    {
        var entity = new CustomerDbEntity
        {
            //Version = 0, -> will default to zero
            CustomerId = "c0",
            FirstName = "John",
            LastName = "Doe"
        };
        
        var result = _runner.MigrateToVersion<CustomerV1>(entity);
        result.CustomerId.Should().Be("c0");
        result.FullName.Should().Be("John Doe");
    }

    [Test]
    public void UpgradeV1_to_V2()
    {
        var entity = new CustomerDbEntity
        {
            Version = 1,
            CustomerId = "c1",
            FullName = "John Doe"
        };
        
        var result = _runner.MigrateToVersion<CustomerV2>(entity);
        result.CustomerId.Should().Be("c1");
        result.FullName.Should().Be("This one -> John Doe");
    }
    

    [Test]
    public void UpgradeV2_to_V3()
    {
        var entity = new CustomerDbEntity
        {
            Version = 2,
            CustomerId = "c2",
            FullName = "This one -> John Doe"
        };
        
        var result = _runner.MigrateToVersion<CustomerV3>(entity);
        result.CustomerId.Should().Be("c2");
        result.FullName.Should().Be("This one -> John Doe");
        result.Age.Should().Be(CustomerV3.DefaultAge);
    }
    
    [Test]
    public void UpgradeV3_to_V4()
    {
        var entity = new CustomerDbEntity
        {
            Version = 3,
            CustomerId = "c3",
            FullName = "John Doe",
            Age = 40
        };
        
        var result = _runner.MigrateToVersion<CustomerV4>(entity);
        result.CustomerId.Should().Be("c3");
        result.FullName.Should().Be("John Doe");
        result.Birthday.Should().Be(DateTime.Today.AddYears(-40));
    }
    
    [Test]
    public void UpgradeV4_to_V4()
    {
        var dob = DateTime.Today.AddYears(-40);
        var entity = new CustomerDbEntity
        {
            Version = 4,
            CustomerId = "c4",
            FullName = "This one -> John Doe",
            DateOfBirth = dob
        };
        
        var result = _runner.MigrateToVersion<CustomerV4>(entity);
        result.CustomerId.Should().Be("c4");
        result.FullName.Should().Be("This one -> John Doe");
        result.Birthday.Should().Be(dob);
    }
    
    [Test]
    public void DowngradeV4_to_V3()
    {
        var dob = DateTime.Today.AddYears(-40);
        var entity = new CustomerDbEntity
        {
            Version = 4,
            CustomerId = "c4",
            FullName = "This one -> John Doe",
            DateOfBirth = dob
        };
        
        var result = _runner.MigrateToVersion<CustomerV3>(entity);
        result.CustomerId.Should().Be("c4");
        result.FullName.Should().Be("This one -> John Doe");
        result.Age.Should().Be(40);
    }
    
    [Test]
    public void DowngradeV3_to_V2()
    {
        var entity = new CustomerDbEntity
        {
            Version = 3,
            CustomerId = "c3",
            FullName = "This one -> John Doe",
            Age = 40
        };
        
        var result = _runner.MigrateToVersion<CustomerV2>(entity);
        result.CustomerId.Should().Be("c3");
        result.FullName.Should().Be("This one -> John Doe");
    }
    
    [Test]
    public void DowngradeV2_to_V1()
    {
        var entity = new CustomerDbEntity
        {
            Version = 2,
            CustomerId = "c2",
            FullName = "This one -> John Doe"
        };
        
        var result = _runner.MigrateToVersion<CustomerV1>(entity);
        result.CustomerId.Should().Be("c2");
        result.FullName.Should().Be("John Doe");
    }
    
    [Test]
    public void DowngradeV1_to_V0()
    {
        var entity = new CustomerDbEntity
        {
            Version = 1,
            CustomerId = "c1",
            FullName = "John Doe"
        };
        
        var result = _runner.MigrateToVersion<CustomerV0>(entity);
        result.CustomerId.Should().Be("c1");
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
    }
}