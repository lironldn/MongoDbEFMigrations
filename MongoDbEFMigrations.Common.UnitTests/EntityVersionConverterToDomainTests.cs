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

public class EntityVersionConverterToDomainTests
{
    private EntityVersionConverter<CustomerDbEntity> _runner;

    [SetUp]
    public void Setup()
    {
        _runner = new EntityVersionConverter<CustomerDbEntity>(
            new List<DbEntityMigratorBase<CustomerDbEntity>>
            {
                new CustomerV0DbEntityMigrator(),
                new CustomerV1DbEntityMigrator(),
                new CustomerV2DbEntityMigrator(),
                new CustomerV3DbEntityMigrator(),
                new CustomerV4DbEntityMigrator()
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
        
        var result = _runner.ToDomain<CustomerV1>(entity);
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
        
        var result = _runner.ToDomain<CustomerV2>(entity);
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
        
        var result = _runner.ToDomain<CustomerV3>(entity);
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
        
        var result = _runner.ToDomain<CustomerV4>(entity);
        result.CustomerId.Should().Be("c3");
        result.FullName.Should().Be("John Doe");
        result.Birthday.Should().Be(DateTime.Today.AddYears(-40));
    }
    
    
    [Test]
    public void UpgradeV0_to_V4()
    {
        var entity = new CustomerDbEntity
        {
            CustomerId = "c0",
            FirstName = "John",
            LastName = "Doe"
        };
        
        var result = _runner.ToDomain<CustomerV4>(entity);
        result.CustomerId.Should().Be("c0");
        result.FullName.Should().Be("This one -> John Doe");
        result.Birthday.Should().Be(DateTime.Today.AddYears(-1 * CustomerV3.DefaultAge));
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
        
        var result = _runner.ToDomain<CustomerV4>(entity);
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
        
        var result = _runner.ToDomain<CustomerV3>(entity);
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
        
        var result = _runner.ToDomain<CustomerV2>(entity);
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
        
        var result = _runner.ToDomain<CustomerV1>(entity);
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
        
        var result = _runner.ToDomain<CustomerV0>(entity);
        result.CustomerId.Should().Be("c1");
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
    }
    
    [Test]
    public void DowngradeV4_to_V0()
    {
        var dob = DateTime.Today.AddYears(-40);
        var entity = new CustomerDbEntity
        {
            Version = 4,
            CustomerId = "c4",
            FullName = "This one -> John Doe",
            DateOfBirth = dob
        };
        
        var result = _runner.ToDomain<CustomerV0>(entity);
        result.CustomerId.Should().Be("c4");
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
    }
    
    [Test]
    public void UpgradeV1_to_V3_WithMissingV2()
    {
        _runner = new EntityVersionConverter<CustomerDbEntity>(
            new List<DbEntityMigratorBase<CustomerDbEntity>>
            {
                new CustomerV1DbEntityMigrator(),
                new CustomerV3DbEntityMigrator(),
            },
            AutoMapperConfig.CreateMapper()
        );
        
        var entity = new CustomerDbEntity
        {
            Version = 1,
            CustomerId = "c1",
            FullName = "John Doe"
        };
        
        var act = () => _runner.ToDomain<CustomerV3>(entity);

        act.Should().Throw<EntityVersionConverterException>()
            .WithMessage("Cannot upgrade from version 1 to Target version 3. Check all Converters are registered.");
    }
    
    [Test]
    public void DownGradeV3_to_V1_WithMissingV2()
    {
        _runner = new EntityVersionConverter<CustomerDbEntity>(
            new List<DbEntityMigratorBase<CustomerDbEntity>>
            {
                new CustomerV1DbEntityMigrator(),
                new CustomerV3DbEntityMigrator(),
            },
            AutoMapperConfig.CreateMapper()
        );
        
        var entity = new CustomerDbEntity
        {
            Version = 3,
            CustomerId = "c3",
            FullName = "This one -> John Doe",
            Age = 40
        };
        
        var act = () => _runner.ToDomain<CustomerV1>(entity);

        act.Should().Throw<EntityVersionConverterException>()
            .WithMessage("Cannot downgrade from version 3 to Target version 1. Check all Converters are registered.");
    }    
    
    [Test]
    public void UpgradeV1_to_V3_WithMissingV3()
    {
        _runner = new EntityVersionConverter<CustomerDbEntity>(
            new List<DbEntityMigratorBase<CustomerDbEntity>>
            {
                new CustomerV1DbEntityMigrator(),
                new CustomerV2DbEntityMigrator(),
            },
            AutoMapperConfig.CreateMapper()
        );
        
        var entity = new CustomerDbEntity
        {
            Version = 1,
            CustomerId = "c1",
            FullName = "John Doe"
        };
        
        var act = () => _runner.ToDomain<CustomerV3>(entity);

        act.Should().Throw<EntityVersionConverterException>()
            .WithMessage("Failed to migrate to version 3. Check all Converters are registered.");
    }
}