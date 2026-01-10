using AutoMapper;
using Swapzy.Application.DTOs.Requests;
using Swapzy.Core.Entities.Users;

namespace Swapzy.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            UserMappings();
        }

        private void UserMappings()
        {
            this.CreateMap<RegisterRequestDto, UserEntity>()
                .ForMember(dest => dest.HashedPassword, opt => opt.Ignore())
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Username));
        }
    }
}
