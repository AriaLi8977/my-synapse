using Microsoft.AspNetCore.Mvc;
using Synapse.Application.DTOs;
using Synapse.Domain.Entities;
using Synapse.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto){
        if (!ModelState.IsValid)
        {
            var errors = string.Join("; ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
            return BadRequest(new AuthResponseDto
            {
                Success = false,
                Code = "VALIDATION_ERROR",
                Message = errors
            });
        }

        var result = await _authService.RegisterAsync(dto);

        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto){
        var result = await _authService.LoginAsync(dto);
        if (!result.Success) return Unauthorized(result);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto){
        if (string.IsNullOrEmpty(dto.RefreshToken))
        {
            return BadRequest(new TokenResponseDto
            {
                Success = false,
                Code = "MISSING_TOKEN",
                Message = "Refresh token is required"
            });
        }

        var result = await _authService.RefreshTokenAsync(dto.RefreshToken);
        if (!result.Success) return Unauthorized(result);
        return Ok(result);
    }
}