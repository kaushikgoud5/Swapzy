using AutoMapper;
using Swapzy.Application.DTOs.Requests;
using Swapzy.Application.DTOs.Responses;
using Swapzy.Core.Entities.Interests;
using Swapzy.Core.Entities.Users;

namespace Swapzy.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterRequestDto, UserEntity>()
            .ForMember(dest => dest.HashedPassword, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Username));

        CreateMap<Interest, InterestResponseDto>()
            .ForMember(dest => dest.ProductTitle, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedOn));
    }
}
