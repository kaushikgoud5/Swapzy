using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Swapzy.Application.DTOs.Responses;
using Swapzy.Application.Interfaces;
using Swapzy.Infrastructure.Data;

namespace Swapzy.Infrastructure.Services
{
    public class ProximityService : IProximityService
    {
        private readonly SwapzyDbContext _context;
        private readonly IStorageService _storageService;

        public ProximityService(SwapzyDbContext context, IStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
        }

        public async Task<List<NearbyProductResponseDto>> GetNearbyProductsAsync(
            double latitude, double longitude, double radiusKm,
            Guid? excludeUserId = null, int page = 1, int pageSize = 20)
        {
            var userLocation = new Point(longitude, latitude) { SRID = 4326 };
            var radiusMeters = radiusKm * 1000;

            var query = _context.Products
                .Include(p => p.Location)
                .Include(p => p.Images.Where(i => i.DateDeleted == null))
                .Where(p => p.DateDeleted == null
                    && p.IsAvailable
                    && p.Location != null
                    && p.Location.GeoLocation != null
                    && p.Location.GeoLocation.IsWithinDistance(userLocation, radiusMeters));

            if (excludeUserId.HasValue)
                query = query.Where(p => p.OwnerId != excludeUserId.Value);

            // exclude products the user already expressed interest in
            if (excludeUserId.HasValue)
            {
                var swipedIds = await _context.Interests
                    .Where(i => i.BuyerId == excludeUserId.Value)
                    .Select(i => i.ProductId)
                    .ToListAsync();

                if (swipedIds.Count > 0)
                    query = query.Where(p => !swipedIds.Contains(p.Id));
            }

            var products = await query
                .OrderBy(p => p.Location!.GeoLocation!.Distance(userLocation))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new
                {
                    p.Id, p.OwnerId, p.Name, p.Description, p.Condition,
                    p.ProductCategoryId, p.EstimatedValue, p.Status, p.IsAvailable, p.CreatedOn,
                    DistanceKm = p.Location!.GeoLocation!.Distance(userLocation) / 1000.0,
                    p.Location,
                    p.Images
                })
                .ToListAsync();

            return products.Select(p => new NearbyProductResponseDto
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
                DistanceKm = p.DistanceKm,
                CreatedOn = p.CreatedOn,
                Location = p.Location == null ? null : new ProductLocationResponseDto
                {
                    Country = p.Location.Country,
                    State = p.Location.State,
                    City = p.Location.City,
                    PostalCode = p.Location.PostalCode,
                    Latitude = p.Location.Latitude,
                    Longitude = p.Location.Longitude
                },
                Images = p.Images
                    .OrderBy(i => i.DisplayOrder)
                    .Select(i => new ProductImageResponseDto
                    {
                        Id = i.Id,
                        Url = _storageService.GetPublicUrl(i.S3Key),
                        DisplayOrder = i.DisplayOrder
                    }).ToList()
            }).ToList();
        }
    }
}
