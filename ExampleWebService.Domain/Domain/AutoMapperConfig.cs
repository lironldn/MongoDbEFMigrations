using AutoMapper;
using ExampleWebService.Domain.Domain.V3;
using ExampleWebService.Domain.Domain.V4;
using ExampleWebService.Domain.Repo;

namespace ExampleWebService.Domain.Domain;

public static class AutoMapperConfig
{
    public static IMapper CreateMapper()
    {
        var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CustomerDbEntity, CustomerV3>();
                cfg.CreateMap<CustomerDbEntity, CustomerV4>()
                    .ForMember(dest => dest.Birthday, opt
                        => opt.MapFrom(src => src.DateOfBirth));
            });

        return mapperConfig.CreateMapper();
    }
}