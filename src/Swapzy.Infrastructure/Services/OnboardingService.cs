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
    public class OnboardingService : IOnboardingService
    {
        private readonly SwapzyDbContext _context;
        private readonly ILogger<OnboardingService> _logger;

        public OnboardingService(SwapzyDbContext context, ILogger<OnboardingService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<OnboardingResponseDto> CompleteOnboardingAsync(OnboardingRequestDto dto, Guid userId)
        {
            var user = await _context.Users
                .Include(u => u.Profile)
                .Include(u => u.PreferredCategories)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new NotFoundException("User not found.");

            if (user.IsOnboarded)
                throw new ConflictException("User has already completed onboarding.");

            var validCategoryIds = await _context.Categories
                .Where(c => dto.PreferredCategoryIds.Contains(c.Id) && c.IsActive && c.DateDeleted == null)
                .Select(c => c.Id)
                .ToListAsync();

            if (validCategoryIds.Count < 3)
                throw new BadRequestException("At least 3 valid active categories are required.");

            try
            {
                if (user.Profile == null)
                {
                    user.Profile = new UserProfile
                    {
                        UserId = userId,
                        AvatarUrl = dto.AvatarUrl,
                        DisplayName = dto.DisplayName,
                        Bio = dto.Bio,
                        Country = dto.Location.Country,
                        State = dto.Location.State,
                        City = dto.Location.City,
                        PostalCode = dto.Location.PostalCode,
                        Latitude = dto.Location.Latitude,
                        Longitude = dto.Location.Longitude,
                        CreatedBy = userId.ToString(),
                        CreatedOn = DateTime.UtcNow
                    };
                }

                var existingCategoryIds = user.PreferredCategories.Select(pc => pc.CategoryId).ToHashSet();
                foreach (var categoryId in validCategoryIds)
                {
                    if (!existingCategoryIds.Contains(categoryId))
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

                user.IsOnboarded = true;
                user.ModifiedOn = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Onboarding completed for user: {UserId}", userId);

                return MapToDto(user, validCategoryIds);
            }
            catch
            {
                throw;
            }
        }

        public async Task<OnboardingResponseDto> GetOnboardingStatusAsync(Guid userId)
        {
            var user = await _context.Users
                .Include(u => u.Profile)
                .Include(u => u.PreferredCategories)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new NotFoundException("User not found.");

            var categoryIds = user.PreferredCategories.Select(pc => pc.CategoryId).ToList();
            return MapToDto(user, categoryIds);
        }

        private static OnboardingResponseDto MapToDto(UserEntity user, List<int> categoryIds)
        {
            var dto = new OnboardingResponseDto
            {
                UserId = user.Id,
                IsOnboarded = user.IsOnboarded,
                AvatarUrl = user.Profile?.AvatarUrl,
                DisplayName = user.Profile?.DisplayName,
                Bio = user.Profile?.Bio,
                PreferredCategoryIds = categoryIds
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
