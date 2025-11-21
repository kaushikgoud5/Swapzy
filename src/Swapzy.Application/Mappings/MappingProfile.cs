using AutoMapper;
using Swapzy.Application.Commands.RegisterUser;
using Swapzy.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swapzy.Application.Mappings
{
	public class MappingProfile : Profile
	{
		public MappingProfile() {
			UserMappings();
		}

		private void UserMappings()
		{
			this.CreateMap<RegisterUserDto, UserEntity>()
				.ForMember(dest => dest.HashedPassword, opt => opt.Ignore())
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Username));
		}
	}
}
