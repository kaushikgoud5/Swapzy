namespace Swapzy.Application.DTOs.Responses
{
    public class UploadUrlResponseDto
    {
        public string UploadUrl { get; set; } = default!;
        public string S3Key { get; set; } = default!;
    }
}
