namespace Swapzy.Application.Interfaces
{
    public interface IStorageService
    {
        Task<string> GenerateUploadUrlAsync(string key, string contentType, int expirationMinutes = 15);
        Task<string> GenerateReadUrlAsync(string key, int expirationMinutes = 60);
        Task DeleteAsync(string key);
    }
}
