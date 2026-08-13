namespace Swapzy.Application.DTOs.Requests;

public class UpdateProfileDto
{
    public string? AvatarUrl { get; set; }
    public string? DisplayName { get; set; }
    public string? Bio { get; set; }
    public LocationDto? Location { get; set; }
    public List<int>? PreferredCategoryIds { get; set; }
}

public class LocationDto
{
    public string Country { get; set; } = default!;
    public string State { get; set; } = default!;
    public string City { get; set; } = default!;
    public string? PostalCode { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
}
