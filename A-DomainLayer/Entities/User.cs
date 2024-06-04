using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace A_DomainLayer.Entities;

public class User : IdentityUser
{
    public bool? IsDeleted { get; set; } = false;
    [MinLength(5)]
    [MaxLength(5)]
    public string? UndoIsDeletedCode { get; set; }

    
        
    public string? ImgUrl { get; set; }
    public string? PublicId { get; set; }

    
    public string? RefreshToken { get; set; } = null!;
    
    public DateTime? RefreshTokenExpiryTime { get; set; } = null!;



}