using AutoMapper;
using MediatR;
using Swapzy.Application.Interfaces;

namespace Swapzy.Application.Commands.LoginUser
{
	public record LoginUserCommand(LoginUserDto loginUserDto) : IRequest<string>;

	public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, string>
	{
		private readonly IUserRepository _userRepository;
		private readonly IAuthService _authService;
		private readonly IMapper _mapper;
		public LoginUserCommandHandler(IUserRepository userRepository, IAuthService authService, IMapper mapper)
		{
			_authService = authService;
			_userRepository = userRepository;
			_mapper = mapper;
		}
		public async Task<string> Handle(LoginUserCommand request, CancellationToken cancellationToken)
		{
			var user = await _userRepository.GetByEmailAsync(request.loginUserDto.Email);
			if (user == null)
				throw new UnauthorizedAccessException("Invalid email or password");

			var isPasswordValid = _authService.VerifyPassword(request.loginUserDto.Password, user.HashedPassword);

			if (!isPasswordValid)
				throw new UnauthorizedAccessException("Invalid email or password");
			var token = _authService.GenerateJwtToken(user);
			return token;
		}

	}
}

