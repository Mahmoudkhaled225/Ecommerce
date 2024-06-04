using System.ComponentModel.DataAnnotations;

namespace D_PresentationLayer.Dtos.AuthDtos;

public class ChangePasswordDto
{
    public string OldPassword { get; set; } = string.Empty;
    
    public string NewPassword { get; set; } = string.Empty;
    
    [Compare("ConfirmPassword", ErrorMessage = "New password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}