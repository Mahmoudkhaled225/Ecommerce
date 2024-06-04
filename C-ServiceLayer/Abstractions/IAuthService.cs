using A_DomainLayer.Entities;
using C_ServiceLayer.Concretes;

namespace C_ServiceLayer.Abstractions;

public interface IAuthService 
{
     string? GetUserIdFromToken(string token);

     string GenerateAccessTokenString(User? user);
     string GenerateRefreshTokenString();
     string GenerateUndoSoftDeleteCode();
     // Task<RefreshTokenDto> RegenerateRefreshToken(string modelAccessToken);
    
}