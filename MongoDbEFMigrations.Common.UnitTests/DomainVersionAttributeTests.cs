using ExampleWebService.Domain.Domain.V0;
using ExampleWebService.Domain.Domain.V1;
using ExampleWebService.Domain.Domain.V2;
using ExampleWebService.Domain.Domain.V3;
using ExampleWebService.Domain.Domain.V4;
using ExampleWebService.Domain.Repo;
using FluentAssertions;

namespace MongoDbEFMigrations.Common.UnitTests;

public class DomainVersionAttributeTests
{
    [Test]
    public void GetVersionWithVersionAttribute_ReturnsVersion()
    {
        DomainVersionAttribute.GetVersion<CustomerV0>().Should().Be(0);
        DomainVersionAttribute.GetVersion<CustomerV1>().Should().Be(1);
        DomainVersionAttribute.GetVersion<CustomerV2>().Should().Be(2);
        DomainVersionAttribute.GetVersion<CustomerV3>().Should().Be(3);
        DomainVersionAttribute.GetVersion<CustomerV4>().Should().Be(4);
    }

    [Test]
    public void GetVersionWithoutVersionAttribute_ThrowsException()
    {
        var action = new Action(() => DomainVersionAttribute.GetVersion<CustomerDbEntity>());
        action.Should().Throw<EntityVersionConverterException>()
            .WithMessage("No DomainVersionAttribute found on type CustomerDbEntity");
    }
}