using AutoMapper;
using Microsoft.Extensions.Logging;
using Swapzy.Application.DTOs.Requests;
using Swapzy.Application.DTOs.Responses;
using Swapzy.Application.Interfaces;
using Swapzy.Core.Constants.Authorization;
using Swapzy.Core.Entities.Users;
using Swapzy.Core.Exceptions;

namespace Swapzy.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ISocialAuthService _socialAuthService;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            IJwtTokenService jwtTokenService,
            ISocialAuthService socialAuthService,
            IMapper mapper,
            ILogger<AuthService> logger)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
            _socialAuthService = socialAuthService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Guid> RegisterAsync(RegisterRequestDto dto)
        {
            _logger.LogInformation("Registering new user with email: {Email}", dto.Email);
            if (await _unitOfWork.Users.EmailExistsAsync(dto.Email))
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

            var userRole = await _unitOfWork.Roles.GetByNameAsync(dto.Role);
            if (userRole == null)
            {
                _logger.LogError("Role not found: {Role}", dto.Role);
                throw new BadRequestException($"Role '{dto.Role}' not found.");
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.Users.AssignRoleAsync(user.Id, userRole);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("User {UserId} registered successfully with role {Role}", user.Id, dto.Role);
                return user.Id;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
        {
            _logger.LogInformation("Login attempt for email: {Email}", dto.Email);

            var user = await _unitOfWork.Users.GetByEmailAsync(dto.Email);
            if (user == null)
            {
                _logger.LogWarning("Login failed: User not found for email {Email}", dto.Email);
                throw new UnauthorizedException("Invalid email or password.");
            }

            var isPasswordValid = _passwordHasher.Verify(dto.Password, user.HashedPassword);
            if (!isPasswordValid)
            {
                _logger.LogWarning("Login failed: Invalid password for user {UserId}", user.Id);
                throw new UnauthorizedException("Invalid email or password.");
            }

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
                User = new AuthUserDto { Id = user.Id.ToString(), Email = user.Email, Name = user.Name },
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

            var isValid = await _jwtTokenService.ValidateRefreshTokenAsync(userId, refreshToken);
            if (!isValid)
            {
                _logger.LogWarning("Refresh token not found or expired for user {UserId}", userId);
                throw new UnauthorizedException("Refresh token expired or revoked.");
            }

            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found or inactive", userId);
                throw new UnauthorizedException("User not found or inactive.");
            }

            await _jwtTokenService.RevokeRefreshTokenAsync(userId, refreshToken);

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
                User = new AuthUserDto { Id = user.Id.ToString(), Email = user.Email, Name = user.Name },
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

        public async Task<AuthResponseDto> SocialLoginAsync(string provider, string token)
        {
            _logger.LogInformation("Social login attempt via {Provider}", provider);

            var socialUser = provider.ToLower() switch
            {
                "google" => await _socialAuthService.ValidateGoogleTokenAsync(token),
                "github" => await _socialAuthService.ValidateGitHubCodeAsync(token),
                _ => throw new BadRequestException($"Unsupported provider: {provider}")
            };

            var user = await _unitOfWork.Users.GetByEmailAsync(socialUser.Email);

            if (user == null)
            {
                // Auto-register
                user = new UserEntity
                {
                    Id = Guid.NewGuid(),
                    Email = socialUser.Email.ToLower(),
                    Name = socialUser.Name,
                    HashedPassword = _passwordHasher.Hash(Guid.NewGuid().ToString()),
                };

                var userRole = await _unitOfWork.Roles.GetByNameAsync(Roles.User);
                await _unitOfWork.BeginTransactionAsync();
                try
                {
                    await _unitOfWork.Users.AddAsync(user);
                    if (userRole != null)
                        await _unitOfWork.Users.AssignRoleAsync(user.Id, userRole);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();
                    _logger.LogInformation("Auto-registered social user {UserId} via {Provider}", user.Id, provider);
                }
                catch
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw;
                }
            }

            var accessToken = await _jwtTokenService.GenerateAccessTokenAsync(user);
            var refreshToken = await _jwtTokenService.GenerateRefreshTokenAsync();
            var expiresAt = DateTime.UtcNow.AddDays(7);
            await _jwtTokenService.StoreRefreshTokenAsync(user.Id, refreshToken, expiresAt);

            _logger.LogInformation("Social login successful for user {UserId} via {Provider}", user.Id, provider);

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt,
                User = new AuthUserDto { Id = user.Id.ToString(), Email = user.Email, Name = user.Name },
            };
        }
    }
}
