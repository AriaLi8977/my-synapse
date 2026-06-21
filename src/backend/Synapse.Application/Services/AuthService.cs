using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Synapse.Domain.Entities;
using Synapse.Application.DTOs;
using Synapse.Application.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;

namespace Synapse.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtSettings _jwtSettings;
    private const int AccessTokenExpiryMinutes = 60; // 1 hour
    private const int RefreshTokenExpiryDays = 7;

    public AuthService(IUserRepository userRepository, IOptions<JwtSettings> jwtOptions)
    {
        _userRepository = userRepository;
        _jwtSettings = jwtOptions.Value;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        if (await _userRepository.ExistsAsync(dto.Email))
        {
            return new AuthResponseDto
            {
                Success = false,
                Code = "EMAIL_EXISTS",
                Message = "Email already in use."
            };
        }

        var user = new Domain.Entities.User
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            Name = dto.Name,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
        };

        await _userRepository.AddAsync(user);
        
        return new AuthResponseDto
        {
            Success = true,
            Code = "Registration Successful",
            Token = GenerateAccessToken(user)
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            return new AuthResponseDto
            {
                Success = false,
                Code = "INVALID_CREDENTIALS",
                Message = "Invalid email or password"
            };
        }

        return new AuthResponseDto
        {
            Success = true,
            Code = "Login Successful",
            Token = GenerateAccessToken(user)
        };
    }

    public async Task<OAuthResponseDto> HandleOAuthCallbackAsync(string provider, string providerId, string email, string name)
    {
        var existingUser = await _userRepository.GetByEmailAsync(email);
        
        if (existingUser != null)
        {
            // Link OAuth to existing account if not already linked
            if (string.IsNullOrEmpty(existingUser.OAuthProvider))
            {
                existingUser.OAuthProvider = provider;
                existingUser.OAuthProviderId = providerId;
                await _userRepository.UpdateAsync(existingUser);
            }
            
            return new OAuthResponseDto
            {
                Success = true,
                Code = "Login Successful",
                Token = GenerateAccessToken(existingUser),
                IsNewUser = false
            };
        }
        
        // Create new user
        var newUser = new Domain.Entities.User
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = name,
            OAuthProvider = provider,
            OAuthProviderId = providerId,
        };

        await _userRepository.AddAsync(newUser);

        return new OAuthResponseDto
        {
            Success = true,
            Code = "Registration Successful",
            Token = GenerateAccessToken(newUser),
            IsNewUser = true
        };
    }

    public async Task<TokenResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
        
        if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
        {
            return new TokenResponseDto
            {
                Success = false,
                Code = "INVALID_TOKEN",
                Message = "Invalid or expired refresh token"
            };
        }

        var newAccessToken = GenerateAccessToken(user);
        var newRefreshToken = GenerateRefreshToken();
        
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(RefreshTokenExpiryDays);
        await _userRepository.UpdateAsync(user);

        return new TokenResponseDto
        {
            Success = true,
            Token = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }

    private string GenerateAccessToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.Name),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim("provider", user.OAuthProvider ?? "local")
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(AccessTokenExpiryMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public async Task<TokenResponseDto> GenerateTokensForUserAsync(User user)
    {
        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();
        
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(RefreshTokenExpiryDays);
        await _userRepository.UpdateAsync(user);

        return new TokenResponseDto
        {
            Success = true,
            Token = accessToken,
            RefreshToken = refreshToken
        };
    }
}