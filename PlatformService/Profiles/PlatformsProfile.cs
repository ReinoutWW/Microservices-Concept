using AutoMapper;
using PlatformService.DTO;
using PlatformService.Models;

namespace PlatformService.Profiles
{
    public class PlatformProfile : Profile
    {
        public PlatformProfile()
        {
            // Source -> Target
            CreateMap<Platform, PlatformReadDTO>();
            CreateMap<PlatformCreateDTO, Platform>();
            CreateMap<PlatformReadDTO, PlatformPublishedDTO>();
            CreateMap<Platform, GrpcPlatformModel>()
                .ForMember(p => p.PlatformId, opt => opt.MapFrom(src => src.Id))
                .ForMember(p => p.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(p => p.Publisher, opt => opt.MapFrom(src => src.Publisher));
        }
    }
}