using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text;
using GateEntry;
using GateEntry.Model;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

[Route("api/[controller]")]
public class AuthController : Controller
{
    private readonly IConfiguration _config;
    private readonly string _adminPin;

    public AuthController(IConfiguration config)
    {
        _config = config;
        _adminPin = _config["Jwt:AdminPin"];
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        if (loginRequest.Pin != _adminPin)
        {
            return Unauthorized(new { message = "Invalid PIN" });
        }

        // If PIN is correct, generate a JWT
        var token = GenerateJwtToken();
        return Ok(new { token });
    }

    private string GenerateJwtToken()
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Add claims. A "Role" claim is good practice.
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, "Admin")
        };

        var expirationInMinutes = _config.GetValue<double>("Jwt:ExpirationInMinutes");

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationInMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}