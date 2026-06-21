namespace Synapse.Application.DTOs;

public class OAuthResponseDto
{
    public bool Success { get; set; }
    public string? Code { get; set; }
    public string? Message { get; set; }
    public string? Token { get; set; }
    public bool IsNewUser { get; set; }
}

public class RefreshTokenDto
{
    public string? RefreshToken { get; set; }
}

public class TokenResponseDto
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public string? Code { get; set; }
    public string? Message { get; set; }
}
