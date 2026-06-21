using Synapse.Application.DTOs;
using Synapse.Domain.Entities;

public interface IAuthService{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto); 
    Task<OAuthResponseDto> HandleOAuthCallbackAsync(string provider, string providerId, string email, string name);
    Task<TokenResponseDto> RefreshTokenAsync(string refreshToken);
    Task<TokenResponseDto> GenerateTokensForUserAsync(User user);
}