namespace Synapse.Domain.Entities;

public class User{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    
    // OAuth fields
    public string? OAuthProvider { get; set; }
    public string? OAuthProviderId { get; set; }
    
    // Security enhancements
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }
}