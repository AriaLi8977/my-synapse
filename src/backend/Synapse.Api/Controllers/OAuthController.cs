using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Synapse.Application.DTOs;
using Synapse.Application.Interfaces;
using System.Security.Claims;

namespace Synapse.Api.Controllers;

[ApiController]
[Route("api/auth/oauth")]
public class OAuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserRepository _userRepository;

    public OAuthController(IAuthService authService, IUserRepository userRepository)
    {
        _authService = authService;
        _userRepository = userRepository;
    }

    [HttpGet("google")]
    public IActionResult GoogleLogin([FromQuery] string? returnUrl = null)
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action(nameof(GoogleCallback), new { returnUrl })
        };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("microsoft")]
    public IActionResult MicrosoftLogin([FromQuery] string? returnUrl = null)
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action(nameof(MicrosoftCallback), new { returnUrl })
        };
        return Challenge(properties, MicrosoftAccountDefaults.AuthenticationScheme);
    }

    [HttpGet("google/callback")]
    public async Task<IActionResult> GoogleCallback([FromQuery] string? returnUrl = null)
    {
        return await HandleOAuthCallback("google", GoogleDefaults.AuthenticationScheme, returnUrl);
    }

    [HttpGet("microsoft/callback")]
    public async Task<IActionResult> MicrosoftCallback([FromQuery] string? returnUrl = null)
    {
        return await HandleOAuthCallback("microsoft", MicrosoftAccountDefaults.AuthenticationScheme, returnUrl);
    }

    private async Task<IActionResult> HandleOAuthCallback(string provider, string scheme, string? returnUrl)
    {
        var authenticateResult = await HttpContext.AuthenticateAsync(scheme);
        
        if (!authenticateResult.Succeeded)
        {
            return Unauthorized(new OAuthResponseDto
            {
                Success = false,
                Code = "OAUTH_FAILED",
                Message = "OAuth authentication failed"
            });
        }

        var email = authenticateResult.Principal?.FindFirst(ClaimTypes.Email)?.Value 
                   ?? authenticateResult.Principal?.FindFirst("email")?.Value;
        var name = authenticateResult.Principal?.FindFirst(ClaimTypes.Name)?.Value 
                  ?? authenticateResult.Principal?.FindFirst("name")?.Value
                  ?? email?.Split('@')[0];
        var providerId = authenticateResult.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? authenticateResult.Principal?.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(email))
        {
            return BadRequest(new OAuthResponseDto
            {
                Success = false,
                Code = "EMAIL_NOT_PROVIDED",
                Message = "Email not provided by OAuth provider"
            });
        }

        var result = await _authService.HandleOAuthCallbackAsync(provider, providerId ?? "", email, name ?? "");
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        // Redirect to frontend with token (for browser-based OAuth flow)
        var frontendUrl = $"{returnUrl ?? "http://localhost:3000"}?token={result.Token}&isNewUser={result.IsNewUser}";
        return Redirect(frontendUrl);
    }
}
