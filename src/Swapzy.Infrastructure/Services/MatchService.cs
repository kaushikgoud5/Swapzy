using Microsoft.EntityFrameworkCore;
using Swapzy.Application.DTOs.Responses;
using Swapzy.Application.Interfaces;
using Swapzy.Core.Entities.Matches;
using Swapzy.Core.Exceptions;
using Swapzy.Infrastructure.Data;

namespace Swapzy.Infrastructure.Services
{
    public class MatchService : IMatchService
    {
        private readonly SwapzyDbContext _context;

        public MatchService(SwapzyDbContext context)
        {
            _context = context;
        }

        public async Task<List<MatchResponseDto>> GetMyMatchesAsync(Guid userId, int page = 1, int pageSize = 20)
        {
            return await _context.Matches
                .Include(m => m.Product)
                .Where(m => (m.InterestedUserId == userId || m.SellerId == userId)
                    && m.DateDeleted == null)
                .OrderByDescending(m => m.CreatedOn)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => MapToDto(m))
                .ToListAsync();
        }

        public async Task<MatchResponseDto> GetByIdAsync(int matchId, Guid userId)
        {
            var match = await _context.Matches
                .Include(m => m.Product)
                .FirstOrDefaultAsync(m => m.Id == matchId
                    && (m.InterestedUserId == userId || m.SellerId == userId)
                    && m.DateDeleted == null)
                ?? throw new NotFoundException($"Match {matchId} not found.");

            return MapToDto(match);
        }

        public async Task<MatchResponseDto> CancelAsync(int matchId, Guid userId)
        {
            var match = await _context.Matches
                .Include(m => m.Product)
                .FirstOrDefaultAsync(m => m.Id == matchId
                    && (m.InterestedUserId == userId || m.SellerId == userId)
                    && m.DateDeleted == null)
                ?? throw new NotFoundException($"Match {matchId} not found.");

            if (match.Status != MatchStatus.Active)
                throw new BadRequestException("Only active matches can be cancelled.");

            match.Status = MatchStatus.Cancelled;
            match.CancelledAt = DateTime.UtcNow;
            match.ModifiedOn = DateTime.UtcNow;
            match.ModifiedBy = userId.ToString();

            await _context.SaveChangesAsync();
            return MapToDto(match);
        }

        private static MatchResponseDto MapToDto(Match m) => new()
        {
            Id = m.Id,
            InterestedUserId = m.InterestedUserId,
            SellerId = m.SellerId,
            ProductId = m.ProductId,
            ProductName = m.Product?.Name ?? string.Empty,
            EstimatedValue = m.Product?.EstimatedValue ?? 0,
            Status = m.Status,
            CreatedOn = m.CreatedOn,
            CancelledAt = m.CancelledAt
        };
    }
}
