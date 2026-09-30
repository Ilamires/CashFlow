using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CashFlow.Api.Contracts.Auth;
using CashFlow.Api.Data;
using CashFlow.Api.Models;
using CashFlow.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ITokenService _tokenService;

    public AuthController(AppDbContext db, ITokenService tokenService)
    {
        _db = db;
        _tokenService = tokenService;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains('@'))
            return BadRequest(new { error = "Некорректный email" });

        if (request.Password is null || request.Password.Length < 8)
            return BadRequest(new { error = "Пароль должен быть не короче 8 символов" });

        string email = request.Email.ToLowerInvariant();

        if (await _db.Users.AnyAsync(u => u.Email == email))
            return Conflict(new { error = "Пользователь с таким email уже существует" });

        User user = new User
        {
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        TokenResult token = _tokenService.CreateToken(user);
        return Created("/api/auth/me", new AuthResponse(token.Token, token.ExpiresAt));
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        string email = request.Email.ToLowerInvariant();
        User? user = await _db.Users.SingleOrDefaultAsync(u => u.Email == email);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized(new { error = "Неверный email или пароль" });

        TokenResult token = _tokenService.CreateToken(user);
        return Ok(new AuthResponse(token.Token, token.ExpiresAt));
    }
    
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        long id = long.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
        User? user = await _db.Users.FindAsync(id);

        if (user is null)
            return NotFound();

        return Ok(new MeResponse(user.Id, user.Email, user.FirstName, user.LastName));
    }
}