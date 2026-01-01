using AutoMapper;
using Swapzy.Application.DTOs;
using Swapzy.Application.Interfaces;
using Swapzy.Core.Entities.Users;

namespace Swapzy.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMapper _mapper;

        public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, IMapper mapper)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
        }

        public async Task<Guid> RegisterAsync(RegisterUserDto dto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                throw new Exception("User with this email already exists.");
            }
            var hashedPassword = _passwordHasher.Hash(dto.Password);
            var user = _mapper.Map<UserEntity>(dto);
            user.HashedPassword = hashedPassword;
            await _userRepository.AddAsync(user);
            return user.Id;
        }

        public async Task<string> LoginAsync(LoginUserDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid email or password");

            var isPasswordValid = _passwordHasher.Verify(dto.Password, user.HashedPassword);

            if (!isPasswordValid)
                throw new UnauthorizedAccessException("Invalid email or password");
            var token = _passwordHasher.GenerateJwtToken(user);
            return token;
        }
    }
}
