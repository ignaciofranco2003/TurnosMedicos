using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TurnosMedicos.Auth.DTOS.Request;
using TurnosMedicos.Auth.DTOS.Response;
using TurnosMedicos.Auth.Entities;
using TurnosMedicos.Auth.Interfaces;
using TurnosMedicos.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TurnosMedicos.Auth.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
    {
        var user = await _db.Users.SingleOrDefaultAsync(u => u.Username == dto.Username);
        if (user is null) throw new UnauthorizedAccessException("Invalid credentials");

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials");

        var token = GenerateToken(user);
        return new AuthResponseDto { Token = token };
    }

    public async Task RegisterAsync(RegisterRequestDto dto)
    {
        if (await _db.Users.AnyAsync(u => u.Username == dto.Username))
            throw new ArgumentException("Username already exists");

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = dto.Role
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
    }

    private string GenerateToken(User user)
    {
        var key = _config["JWT:SECRET"];
        var issuer = _config["JWT:ISSUER"];
        var audience = _config["JWT:AUDIENCE"];
        if (string.IsNullOrEmpty(key)) throw new InvalidOperationException("JWT secret not configured");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        // Token expiration in minutes (configurable via JWT:EXPIRES_MINUTES). Default = 8 hours.
        var expiresMinutes = 8 * 60;
        var configured = _config["JWT:EXPIRES_MINUTES"];
        if (!string.IsNullOrEmpty(configured) && int.TryParse(configured, out var parsedMinutes))
        {
            expiresMinutes = parsedMinutes;
        }

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
