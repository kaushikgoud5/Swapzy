namespace Swapzy.Application.DTOs.Responses
{
    public class ProductImageResponseDto
    {
        public int Id { get; set; }
        public string Url { get; set; } = default!;
        public int DisplayOrder { get; set; }
    }
}
