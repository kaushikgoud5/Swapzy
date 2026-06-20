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

        public ProximityService(SwapzyDbContext context)
        {
            _context = context;
        }

        public async Task<List<NearbyProductResponseDto>> GetNearbyProductsAsync(
            double latitude, double longitude, double radiusKm,
            Guid? excludeUserId = null, int page = 1, int pageSize = 20)
        {
            var userLocation = new Point(longitude, latitude) { SRID = 4326 };
            var radiusMeters = radiusKm * 1000;

            var query = _context.Products
                .Include(p => p.Location)
                .Where(p => p.DateDeleted == null
                    && p.IsAvailable
                    && p.Location != null
                    && p.Location.GeoLocation != null
                    && p.Location.GeoLocation.IsWithinDistance(userLocation, radiusMeters));

            if (excludeUserId.HasValue)
                query = query.Where(p => p.OwnerId != excludeUserId.Value);

            var products = await query
                .OrderBy(p => p.Location!.GeoLocation!.Distance(userLocation))
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

            return products;
        }
    }
}
