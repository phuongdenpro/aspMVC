namespace CMVC.Controllers;

using CMVC.DTOs;
using CMVC.Helpers.Sercurity;
using CMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly AppDbContext _context;
    private readonly IJwtService _jwtService;
    public AuthController(
        IConfiguration config,
        AppDbContext context,
        IJwtService jwtService)
    {
        _config = config;
        _context = context;
        _jwtService = jwtService;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    public IActionResult AdminOnly()
    {
        return Ok("Admin access");
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var user = new User
        {
            Username = dto.Username,
            PasswordHash = PasswordHelper.Hash(dto.Password),
            Role = "User"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok("Register success");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var hash = PasswordHelper.Hash(dto.Password);

        var user = _context.Users.FirstOrDefault(x =>
            x.Username == dto.Username && x.PasswordHash == hash);

        if (user == null)
            return Unauthorized();

        var accessToken = _jwtService.CreateAccessToken(user, _config);

        var refreshToken = new RefreshToken
        {
            Token = new Helpers.Sercurity.JwtHelper().GenerateRefreshToken(),
            ExpiryDate = DateTime.UtcNow.AddDays(7),
            UserId = user.Id,
            IsRevoked = false
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            accessToken,
            refreshToken = refreshToken.Token
        });
    }

    [HttpPost("refresh")]
    public IActionResult Refresh(string refreshToken)
    {
        var token = _context.RefreshTokens
            .FirstOrDefault(x => x.Token == refreshToken && !x.IsRevoked);

        if (token == null || token.ExpiryDate < DateTime.UtcNow)
            return Unauthorized();

        var user = _context.Users.Find(token.UserId);

        var newAccessToken = _jwtService.CreateAccessToken(user, _config);

        return Ok(new
        {
            accessToken = newAccessToken
        });
    }

    [HttpPost("logout")]
    public IActionResult Logout(string refreshToken)
    {
        var token = _context.RefreshTokens.FirstOrDefault(x => x.Token == refreshToken);

        if (token != null)
        {
            token.IsRevoked = true;
            _context.SaveChanges();
        }

        return Ok("Logged out");
    }

    //[HttpPost("login")]
    //public IActionResult Login(LoginDto dto)
    //{
    //    if (dto.Username != "admin" || dto.Password != "123456")
    //        return Unauthorized();

    //    var tokenHandler = new JwtSecurityTokenHandler();
    //    var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);

    //    var tokenDescriptor = new SecurityTokenDescriptor
    //    {
    //        Subject = new ClaimsIdentity(new[]
    //        {
    //        new Claim(ClaimTypes.Name, dto.Username)
    //    }),
    //        Expires = DateTime.UtcNow.AddHours(1),
    //        Issuer = _config["Jwt:Issuer"],
    //        Audience = _config["Jwt:Audience"],
    //        SigningCredentials = new SigningCredentials(
    //            new SymmetricSecurityKey(key),
    //            SecurityAlgorithms.HmacSha256Signature)
    //    };

    //    var token = tokenHandler.CreateToken(tokenDescriptor);
    //    return Ok(new
    //    {
    //        token = tokenHandler.WriteToken(token)
    //    });
    //}
}



