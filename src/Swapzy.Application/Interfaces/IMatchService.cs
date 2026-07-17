using Swapzy.Application.DTOs.Responses;

namespace Swapzy.Application.Interfaces
{
    public interface IMatchService
    {
        Task<List<MatchResponseDto>> GetMyMatchesAsync(Guid userId, int page = 1, int pageSize = 20);
        Task<MatchResponseDto> GetByIdAsync(int matchId, Guid userId);
        Task<MatchResponseDto> CancelAsync(int matchId, Guid userId);
    }
}
