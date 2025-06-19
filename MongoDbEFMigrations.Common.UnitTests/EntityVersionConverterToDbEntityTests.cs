using ExampleWebService.Domain.Domain;
using ExampleWebService.Domain.Domain.V0;
using ExampleWebService.Domain.Domain.V1;
using ExampleWebService.Domain.Domain.V2;
using ExampleWebService.Domain.Domain.V3;
using ExampleWebService.Domain.Domain.V4;
using ExampleWebService.Domain.Repo;
using FluentAssertions;

namespace MongoDbEFMigrations.Common.UnitTests;

public class EntityVersionConverterToDbEntityTests
{
    private EntityVersionConverter<CustomerDbEntity> _runner;

    [SetUp]
    public void Setup()
    {
        _runner = new EntityVersionConverter<CustomerDbEntity>(
            new List<DbEntityMigratorBase<CustomerDbEntity>>(), // not used anyway
            AutoMapperConfig.CreateMapper()
        );
    }

    [Test]
    public void V0_to_DbEntity()
    {
        var domainObject = new CustomerV0
        {
            CustomerId = "c0",
            FirstName = "John",
            LastName = "Doe"
        };

        var result = _runner.ToDbEntity(domainObject);
        result.Version.Should().Be(0);
        result.CustomerId.Should().Be("c0");
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
        result.FullName.Should().BeNull();
        result.Age.Should().BeNull();
        result.DateOfBirth.Should().BeNull();
    }

    [Test]
    public void V1_to_DbEntity()
    {
        var domainObject = new CustomerV1
        {
            CustomerId = "c1",
            FullName = "John Doe"
        };

        var result = _runner.ToDbEntity(domainObject);
        result.Version.Should().Be(1);
        result.CustomerId.Should().Be("c1");
        result.FirstName.Should().BeNull();
        result.LastName.Should().BeNull();
        result.FullName.Should().Be("John Doe");
        result.Age.Should().BeNull();
        result.DateOfBirth.Should().BeNull();
    }

    [Test]
    public void V2_to_DbEntity()
    {
        var domainObject = new CustomerV2
        {
            CustomerId = "c2",
            FullName = "This one -> John Doe"
        };

        var result = _runner.ToDbEntity(domainObject);
        result.Version.Should().Be(2);
        result.CustomerId.Should().Be("c2");
        result.FirstName.Should().BeNull();
        result.LastName.Should().BeNull();
        result.FullName.Should().Be("This one -> John Doe");
        result.Age.Should().BeNull();
        result.DateOfBirth.Should().BeNull();
    }

    [Test]
    public void V3_to_DbEntity()
    {
        var domainObject = new CustomerV3
        {
            CustomerId = "c3",
            FullName = "This one -> John Doe",
            Age = 33
        };

        var result = _runner.ToDbEntity(domainObject);
        result.Version.Should().Be(3);
        result.CustomerId.Should().Be("c3");
        result.FirstName.Should().BeNull();
        result.LastName.Should().BeNull();
        result.FullName.Should().Be("This one -> John Doe");
        result.Age.Should().Be(33);
        result.DateOfBirth.Should().BeNull();
    }

    [Test]
    public void V4_to_DbEntity()
    {
        var domainObject = new CustomerV4
        {
            CustomerId = "c4",
            FullName = "This one -> John Doe",
            Birthday = DateTime.Today.AddYears(-20)
        };

        var result = _runner.ToDbEntity(domainObject);
        result.Version.Should().Be(4);
        result.CustomerId.Should().Be("c4");
        result.FirstName.Should().BeNull();
        result.LastName.Should().BeNull();
        result.FullName.Should().Be("This one -> John Doe");
        result.Age.Should().BeNull();
        result.DateOfBirth.Should().Be(domainObject.Birthday);
    }
}