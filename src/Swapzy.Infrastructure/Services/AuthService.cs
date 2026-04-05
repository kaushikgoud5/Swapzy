using AutoMapper;
using Microsoft.Extensions.Logging;
using Swapzy.Application.DTOs.Requests;
using Swapzy.Application.DTOs.Responses;
using Swapzy.Application.Interfaces;
using Swapzy.Core.Entities.Users;
using Swapzy.Core.Exceptions;

namespace Swapzy.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenService jwtTokenService,
            IMapper mapper,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Guid> RegisterAsync(RegisterRequestDto dto)
        {
            _logger.LogInformation("Registering new user with email: {Email}", dto.Email);
            if (await _userRepository.EmailExistsAsync(dto.Email))
            {
                _logger.LogWarning("Registration failed: Email {Email} already exists", dto.Email);
                throw new ConflictException("User with this email already exists.");
            }
            var hashedPassword = _passwordHasher.Hash(dto.Password);
            var user = new UserEntity
            {
                Email = dto.Email.ToLower(),
                Name = dto.Username,
                HashedPassword = hashedPassword,
            };
            var createdUser = await _userRepository.AddAsync(user);
            _logger.LogInformation("User created with ID: {UserId}", createdUser.Id);
            var userRole = await _roleRepository.GetByNameAsync(dto.Role);
            if (userRole == null)
            {
                _logger.LogError("Role not found: {Role}", dto.Role);
                throw new BadRequestException($"Role '{dto.Role}' not found.");
            }
            await _userRepository.AssignRoleAsync(createdUser.Id, userRole);
            _logger.LogInformation("Assigned role {Role} to user {UserId}", dto.Role, createdUser.Id);

            _logger.LogInformation("User {UserId} registered successfully", createdUser.Id);

            return createdUser.Id;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
        {
            _logger.LogInformation("Login attempt for email: {Email}", dto.Email);

            // Find user
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null)
            {
                _logger.LogWarning("Login failed: User not found for email {Email}", dto.Email);
                throw new UnauthorizedException("Invalid email or password.");
            }

            // Verify password
            var isPasswordValid = _passwordHasher.Verify(dto.Password, user.HashedPassword);
            if (!isPasswordValid)
            {
                _logger.LogWarning("Login failed: Invalid password for user {UserId}", user.Id);
                throw new UnauthorizedException("Invalid email or password.");
            }

            // Generate tokens
            var accessToken = await _jwtTokenService.GenerateAccessTokenAsync(user);
            var refreshToken = await _jwtTokenService.GenerateRefreshTokenAsync();
            var expiresAt = DateTime.UtcNow.AddDays(7);

            await _jwtTokenService.StoreRefreshTokenAsync(user.Id, refreshToken, expiresAt);
            _logger.LogInformation("User {UserId} logged in successfully", user.Id);

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt,
            };
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
        {
            _logger.LogInformation("Refresh token request received");

            var principal = _jwtTokenService.ValidateToken(refreshToken);
            if (principal == null)
            {
                _logger.LogWarning("Refresh token validation failed");
                throw new UnauthorizedException("Invalid refresh token.");
            }

            var userIdClaim = principal.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                _logger.LogWarning("Invalid user ID in refresh token");
                throw new UnauthorizedException("Invalid token claims.");
            }

            // Validate refresh token exists in cache
            var isValid = await _jwtTokenService.ValidateRefreshTokenAsync(userId, refreshToken);
            if (!isValid)
            {
                _logger.LogWarning("Refresh token not found or expired for user {UserId}", userId);
                throw new UnauthorizedException("Refresh token expired or revoked.");
            }

            // Get user
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found or inactive", userId);
                throw new UnauthorizedException("User not found or inactive.");
            }

            // Revoke old refresh token
            await _jwtTokenService.RevokeRefreshTokenAsync(userId, refreshToken);

            // Generate new tokens
            var newAccessToken = await _jwtTokenService.GenerateAccessTokenAsync(user);
            var newRefreshToken = await _jwtTokenService.GenerateRefreshTokenAsync();
            var expiresAt = DateTime.UtcNow.AddDays(7);

            await _jwtTokenService.StoreRefreshTokenAsync(user.Id, newRefreshToken, expiresAt);

            _logger.LogInformation("Tokens refreshed for user {UserId}", userId);

            return new AuthResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = expiresAt,
            };
           }

           public async Task<bool> LogoutAsync(Guid userId, string refreshToken)
           {
               _logger.LogInformation("Logout request for user {UserId}", userId);

               await _jwtTokenService.RevokeRefreshTokenAsync(userId, refreshToken);

               _logger.LogInformation("User {UserId} logged out successfully", userId);
               return true;
           }

           public async Task<bool> RevokeAllTokensAsync(Guid userId)
           {
               _logger.LogInformation("Revoking all tokens for user {UserId}", userId);

               await _jwtTokenService.RevokeAllUserTokensAsync(userId);

               _logger.LogInformation("All tokens revoked for user {UserId}", userId);
               return true;
           }
        }
    }
