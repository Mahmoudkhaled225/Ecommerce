using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using A_DomainLayer.Entities;
using A_DomainLayer.Interfaces;
using C_ServiceLayer.Abstractions;
using C_ServiceLayer.Concretes;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace C_ServiceLayer.Utils;

public class AuthService : IAuthService
{
    private readonly Jwt _jwt;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AuthService(IOptions<Jwt> jwt, IUnitOfWork unitOfWork, UserManager<User> userManager,
        SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
    {
        _jwt = jwt.Value;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }

    private ClaimsPrincipal GetTokenPrincipal(string token)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Secret));
        var validation = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = securityKey,
            ValidateIssuer = true,
            ValidIssuer = _jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = _jwt.Audience,
            RequireExpirationTime = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
        return new JwtSecurityTokenHandler().ValidateToken(token, validation, out _);
    }
    public string? GetUserIdFromToken(string token)
    {
        var principal = GetTokenPrincipal(token);
        var userId = principal?.Claims?.FirstOrDefault(c => c.Type == "Id")?.Value;
        return userId;
    }
    public string GenerateAccessTokenString(User? dto)
    {
        var claims = new List<Claim>
        {
            new (ClaimTypes.Role, _userManager.GetRolesAsync(dto).Result.FirstOrDefault() ?? "User"),
            new Claim(ClaimTypes.NameIdentifier, dto.Id),
        };
        
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Secret));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwt.Issuer,
            Audience = _jwt.Audience,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(_jwt.DurationInHours),
            SigningCredentials = signingCredentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    public string GenerateRefreshTokenString()
    {
        var randomNumber = new byte[64];
        using (var numberGenerator = RandomNumberGenerator.Create())
            numberGenerator.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
    
    public string GenerateUndoSoftDeleteCode()
    {
        var random = new Random();
        return random.Next(10000, 99999).ToString();
    }

    
    // public async Task<RefreshTokenDto> RegenerateRefreshToken(string accessToken)
    // {
    //     var userId = GetUserIdFromToken(accessToken);
    //     if (userId == null)
    //         throw new SecurityTokenException("Invalid token");
    //     
    //
    //     var user = await _userManager.FindByIdAsync(userId);
    //     if (user == null)
    //         throw new SecurityTokenException("User not found");
    //     
    //     
    //     // Generate a new access token and refresh token
    //     var newAccessToken = GenerateAccessTokenString(user);
    //     var newRefreshToken = GenerateRefreshTokenString();
    //     
    //     return new RefreshTokenDto { AccessToken = newAccessToken, RefreshToken = newRefreshToken };
    // }

    
}
     
     
