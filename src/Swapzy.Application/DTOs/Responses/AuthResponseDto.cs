namespace Swapzy.Application.DTOs.Responses;

public class AuthResponseDto
{
    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
    public AuthUserDto User { get; set; } = default!;
}

public class AuthUserDto
{
    public string Id { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Name { get; set; } = default!;
}
