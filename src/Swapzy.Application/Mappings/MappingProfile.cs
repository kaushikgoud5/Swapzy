using AutoMapper;
using Swapzy.Application.DTOs;
using Swapzy.Core.Entities;

namespace Swapzy.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
         //   UserMappings();
        }

/*        private void UserMappings()
        {
            this.CreateMap<RegisterUserDto, UserEntity>()
                .ForMember(dest => dest.HashedPassword, opt => opt.Ignore())
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Username));
        }*/
    }
}
