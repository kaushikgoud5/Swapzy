using System.ComponentModel.DataAnnotations;

namespace Swapzy.Application.DTOs.Requests
{
    public class ConfirmImageDto
    {
        [Required]
        public string S3Key { get; set; } = default!;
        public string ContentType { get; set; } = "image/jpeg";
    }
}
