using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CashFlow.Api.Models;
using Microsoft.IdentityModel.Tokens;

namespace CashFlow.Api.Services;

public record TokenResult(string Token, DateTimeOffset ExpiresAt);

public interface ITokenService
{
    TokenResult CreateToken(User user);
}

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public TokenResult CreateToken(User user)
    {
        SymmetricSecurityKey key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

        DateTimeOffset expiresAt = DateTimeOffset.UtcNow.AddMinutes(
            _configuration.GetValue("Jwt:ExpiresMinutes", 60));

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims:
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            ],
            expires: expiresAt.UtcDateTime,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new TokenResult(
            new JwtSecurityTokenHandler().WriteToken(token),
            expiresAt);
    }
}