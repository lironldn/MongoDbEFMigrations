using AutoMapper;
using ExampleWebService.Domain.Domain.V0;
using ExampleWebService.Domain.Domain.V1;
using ExampleWebService.Domain.Domain.V2;
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