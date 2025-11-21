using AutoMapper;
using MediatR;
using Microsoft.AspNet.Identity;
using Swapzy.Application.Interfaces;
using Swapzy.Core.Entities;
using System.Collections.Generic;

namespace Swapzy.Application.Commands.RegisterUser
{
	public record RegisterUserCommand(RegisterUserDto userModel) : IRequest<Guid>;

	public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
	{
		private readonly IUserRepository _userRepository;
		private readonly IAuthService _authService;
		private readonly IMapper _mapper;
		public RegisterUserCommandHandler(IUserRepository userRepository, IAuthService authService, IMapper mapper)
		{
			_userRepository = userRepository;
			_authService = authService;
			_mapper = mapper;
		}
		public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
		{
			var hashedPassword = _authService.HashPassword(request.userModel.Password);
			var user = new UserEntity { HashedPassword = hashedPassword, Name = request.userModel.Username, Email = request.userModel.Email};
			var x =  await _userRepository.AddAsync(user);
			return x.Id;
		}
	}
}
