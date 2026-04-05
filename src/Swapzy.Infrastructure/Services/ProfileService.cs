using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Swapzy.Application.DTOs.Requests;
using Swapzy.Application.DTOs.Responses;
using Swapzy.Application.Interfaces;
using Swapzy.Core.Entities.Categories;
using Swapzy.Core.Entities.Users;
using Swapzy.Core.Exceptions;
using Swapzy.Infrastructure.Data;

namespace Swapzy.Infrastructure.Services
{
    public class ProfileService : IProfileService
    {
        private readonly SwapzyDbContext _context;
        private readonly ILogger<ProfileService> _logger;

        public ProfileService(SwapzyDbContext context, ILogger<ProfileService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ProfileResponseDto> GetProfileAsync(Guid userId)
        {
            var user = await _context.Users
                .Include(u => u.Profile)
                .Include(u => u.PreferredCategories)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new NotFoundException("User not found.");

            return MapToDto(user);
        }

        public async Task<ProfileResponseDto> UpdateProfileAsync(UpdateProfileDto dto, Guid userId)
        {
            var user = await _context.Users
                .Include(u => u.Profile)
                .Include(u => u.PreferredCategories)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new NotFoundException("User not found.");

            if (user.Profile == null)
                throw new BadRequestException("Complete onboarding before editing profile.");

            // Update profile fields (only if provided)
            if (dto.AvatarUrl != null)
                user.Profile.AvatarUrl = dto.AvatarUrl;

            if (dto.DisplayName != null)
                user.Profile.DisplayName = dto.DisplayName;

            if (dto.Bio != null)
                user.Profile.Bio = dto.Bio;

            if (dto.Location != null)
            {
                user.Profile.Country = dto.Location.Country;
                user.Profile.State = dto.Location.State;
                user.Profile.City = dto.Location.City;
                user.Profile.PostalCode = dto.Location.PostalCode;
                user.Profile.Latitude = dto.Location.Latitude;
                user.Profile.Longitude = dto.Location.Longitude;
            }

            // Update preferred categories if provided
            if (dto.PreferredCategoryIds != null)
            {
                if (dto.PreferredCategoryIds.Count < 3)
                    throw new BadRequestException("At least 3 preferred categories are required.");

                var validCategoryIds = await _context.Categories
                    .Where(c => dto.PreferredCategoryIds.Contains(c.Id) && c.IsActive && c.DateDeleted == null)
                    .Select(c => c.Id)
                    .ToListAsync();

                if (validCategoryIds.Count < 3)
                    throw new BadRequestException("At least 3 valid active categories are required.");

                // Remove old, add new
                var existing = user.PreferredCategories.ToList();
                _context.UserPreferredCategories.RemoveRange(existing);

                foreach (var categoryId in validCategoryIds)
                {
                    _context.UserPreferredCategories.Add(new UserPreferredCategory
                    {
                        UserId = userId,
                        CategoryId = categoryId,
                        CreatedBy = userId.ToString(),
                        CreatedOn = DateTime.UtcNow
                    });
                }
            }

            user.Profile.ModifiedBy = userId.ToString();
            user.Profile.ModifiedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Profile updated for user: {UserId}", userId);

            // Re-fetch to get updated categories
            await _context.Entry(user).Collection(u => u.PreferredCategories).LoadAsync();
            return MapToDto(user);
        }

        private static ProfileResponseDto MapToDto(UserEntity user)
        {
            var dto = new ProfileResponseDto
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                AvatarUrl = user.Profile?.AvatarUrl,
                DisplayName = user.Profile?.DisplayName,
                Bio = user.Profile?.Bio,
                PreferredCategoryIds = user.PreferredCategories.Select(pc => pc.CategoryId).ToList()
            };

            if (user.Profile is { Country: not null })
            {
                dto.Location = new OnboardingLocationResponseDto
                {
                    Country = user.Profile.Country,
                    State = user.Profile.State!,
                    City = user.Profile.City!,
                    PostalCode = user.Profile.PostalCode,
                    Latitude = user.Profile.Latitude,
                    Longitude = user.Profile.Longitude
                };
            }

            return dto;
        }
    }
}
