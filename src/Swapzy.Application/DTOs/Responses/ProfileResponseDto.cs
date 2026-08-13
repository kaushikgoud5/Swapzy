namespace Swapzy.Application.DTOs.Responses;

public class ProfileResponseDto
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? AvatarUrl { get; set; }
    public string? DisplayName { get; set; }
    public string? Bio { get; set; }
    public LocationResponseDto? Location { get; set; }
    public List<int> PreferredCategoryIds { get; set; } = new();
}

public class LocationResponseDto
{
    public string Country { get; set; } = default!;
    public string State { get; set; } = default!;
    public string City { get; set; } = default!;
    public string? PostalCode { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
}
