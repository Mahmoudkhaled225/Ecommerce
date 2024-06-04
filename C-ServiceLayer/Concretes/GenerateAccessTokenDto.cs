using Microsoft.AspNetCore.Identity;

namespace C_ServiceLayer.Concretes;

public class GenerateAccessTokenDto
{
    public string Id { get; set; }

    public IdentityRole Role { get; set; }

}