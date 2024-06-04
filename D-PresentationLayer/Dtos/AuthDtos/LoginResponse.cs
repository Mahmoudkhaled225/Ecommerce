namespace D_PresentationLayer.Dtos.AuthDtos;

public class LoginResponse
{
    // message with default value which is LogIn successful
    public string Message { get; set; } = "LogIn successful";
    // access token and refresh token
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; } 
}