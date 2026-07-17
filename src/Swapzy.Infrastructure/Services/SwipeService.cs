using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using Swapzy.Application.DTOs.Requests;
using Swapzy.Application.DTOs.Responses;
using Swapzy.Application.Interfaces;
using Swapzy.Core.Entities.Matches;
using Swapzy.Core.Entities.Swipes;
using Swapzy.Core.Enums;
using Swapzy.Core.Events;
using Swapzy.Core.Exceptions;
using Swapzy.Infrastructure.Data;

namespace Swapzy.Infrastructure.Services
{
    public class SwipeService : ISwipeService
    {
        private readonly SwapzyDbContext _context;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<SwipeService> _logger;

        public SwipeService(SwapzyDbContext context, IEventPublisher eventPublisher, ILogger<SwipeService> logger)
        {
            _context = context;
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        public async Task<BatchSwipeResponseDto> BatchSwipeAsync(BatchSwipeRequestDto dto, Guid swiperId)
        {
            if (dto.Swipes == null || dto.Swipes.Count == 0)
                throw new BadRequestException("No swipes provided.");

            if (dto.Swipes.Count > 50)
                throw new BadRequestException("Maximum 50 swipes per batch.");

            var productIds = dto.Swipes.Select(s => s.ProductId).Distinct().ToList();

            // Fix #2: include Name in projection
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id) && p.DateDeleted == null)
                .Select(p => new { p.Id, p.OwnerId, p.IsAvailable, p.Name })
                .ToDictionaryAsync(p => p.Id);

            var existingSwipes = await _context.Swipes
                .Where(s => s.SwiperId == swiperId && productIds.Contains(s.ProductId))
                .ToDictionaryAsync(s => s.ProductId);

            var newSwipes = new List<Swipe>();
            var likedProductIds = new List<int>();

            foreach (var item in dto.Swipes)
            {
                if (!products.TryGetValue(item.ProductId, out var product)) continue;
                if (product.OwnerId == swiperId) continue;
                if (!product.IsAvailable) continue;

                if (existingSwipes.TryGetValue(item.ProductId, out var existing))
                {
                    existing.Direction = item.Direction;
                    existing.ModifiedOn = DateTime.UtcNow;
                }
                else
                {
                    newSwipes.Add(new Swipe
                    {
                        SwiperId = swiperId,
                        ProductId = item.ProductId,
                        Direction = item.Direction,
                        CreatedOn = DateTime.UtcNow,
                        CreatedBy = swiperId.ToString()
                    });
                }

                if (item.Direction == SwipeDirection.Like)
                    likedProductIds.Add(item.ProductId);
            }

            if (newSwipes.Count > 0)
                await _context.Swipes.AddRangeAsync(newSwipes);

            await _context.SaveChangesAsync();

            // Fix #3: one-sided match — no mutual check needed
            // Fix #1: load existing matches in ONE query, bulk insert
            var matchDtos = new List<SwipeMatchDto>();

            if (likedProductIds.Count > 0)
            {
                // Check which liked products already have a match to avoid duplicates
                var alreadyMatchedProductIds = (await _context.Matches
                    .Where(x => x.InterestedUserId == swiperId && likedProductIds.Contains(x.ProductId))
                    .Select(x => x.ProductId)
                    .ToListAsync()).ToHashSet();

                var newMatches = likedProductIds
                    .Where(pid => !alreadyMatchedProductIds.Contains(pid))
                    .Select(pid => new Match
                    {
                        InterestedUserId = swiperId,
                        SellerId = products[pid].OwnerId,
                        ProductId = pid,
                        Status = MatchStatus.Active,
                        CreatedOn = DateTime.UtcNow,
                        CreatedBy = swiperId.ToString()
                    })
                    .ToList();

                if (newMatches.Count > 0)
                {
                    // Fix #1: single bulk insert + single SaveChanges
                    await _context.Matches.AddRangeAsync(newMatches);
                    await _context.SaveChangesAsync();

                    foreach (var match in newMatches)
                    {
                        matchDtos.Add(new SwipeMatchDto
                        {
                            ProductId = match.ProductId,
                            MatchedWithUserId = match.SellerId
                        });

                        try
                        {
                            await _eventPublisher.PublishAsync(new MatchCreatedEvent
                            {
                                MatchId = match.Id,
                                InterestedUserId = swiperId,
                                SellerId = match.SellerId,
                                ProductId = match.ProductId,
                                // Fix #2: correct ProductName
                                ProductName = products[match.ProductId].Name
                            });
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to publish MatchCreatedEvent for match {MatchId}", match.Id);
                        }
                    }
                }
            }

            _logger.LogInformation("User {UserId} batch swiped {Count} items, {Matches} new matches",
                swiperId, dto.Swipes.Count, matchDtos.Count);

            return new BatchSwipeResponseDto
            {
                Processed = dto.Swipes.Count,
                Matches = matchDtos
            };
        }

        // Fix #9: removed duplicate proximity logic — feed uses same PostGIS query
        // but adds category preference weighting and hasMore
        public async Task<List<NearbyProductResponseDto>> GetFeedAsync(
            double latitude, double longitude, double radiusKm,
            Guid userId, int page = 1, int pageSize = 20)
        {
            var userLocation = new Point(longitude, latitude) { SRID = 4326 };
            var radiusMeters = radiusKm * 1000;

            // Fix #5: load user preferred categories
            var preferredCategoryIds = (await _context.UserPreferredCategories
                .Where(x => x.UserId == userId)
                .Select(x => x.CategoryId)
                .ToListAsync()).ToHashSet();

            var alreadySwiped = _context.Swipes
                .Where(s => s.SwiperId == userId)
                .Select(s => s.ProductId);

            return await _context.Products
                .Include(p => p.Location)
                .Where(p => p.DateDeleted == null
                    && p.IsAvailable
                    && p.OwnerId != userId
                    && p.Location != null
                    && p.Location.GeoLocation != null
                    && p.Location.GeoLocation.IsWithinDistance(userLocation, radiusMeters)
                    && !alreadySwiped.Contains(p.Id))
                // Fix #5: preferred categories first, then by distance
                .OrderBy(p => preferredCategoryIds.Contains(p.ProductCategoryId) ? 0 : 1)
                .ThenBy(p => p.Location!.GeoLocation!.Distance(userLocation))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new NearbyProductResponseDto
                {
                    Id = p.Id,
                    OwnerId = p.OwnerId,
                    Name = p.Name,
                    Description = p.Description,
                    Condition = p.Condition,
                    ProductCategoryId = p.ProductCategoryId,
                    EstimatedValue = p.EstimatedValue,
                    Status = p.Status,
                    IsAvailable = p.IsAvailable,
                    DistanceKm = p.Location!.GeoLocation!.Distance(userLocation) / 1000.0,
                    CreatedOn = p.CreatedOn,
                    Location = new ProductLocationResponseDto
                    {
                        Country = p.Location.Country,
                        State = p.Location.State,
                        City = p.Location.City,
                        PostalCode = p.Location.PostalCode,
                        Latitude = p.Location.Latitude,
                        Longitude = p.Location.Longitude
                    }
                })
                .ToListAsync();
        }
    }
}
