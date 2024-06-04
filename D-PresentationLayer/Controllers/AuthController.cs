using System.Security.Claims;
using A_DomainLayer.Entities;
using AutoMapper;
using C_ServiceLayer.Abstractions;
using C_ServiceLayer.Concretes;
using D_PresentationLayer.Dtos.AuthDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace D_PresentationLayer.Controllers;

public class AuthController : BaseController
{

    private readonly IMapper _mapper;
    private readonly ILogger _logger;
    private readonly ISmsService _smsService;
    private readonly IAuthService _authService;
    private readonly IEmailService _emailService;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    

    
    public AuthController(IMapper mapper, ILogger<AuthController> logger, ISmsService smsService, 
        IAuthService authService, IEmailService emailService, UserManager<User> userManager, 
        SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
    {
        _mapper = mapper;
        _logger = logger;
        _smsService = smsService;
        _authService = authService;
        _emailService = emailService;
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }    
    private async Task<User?> ValidateUser(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null)
        {
            _logger.LogWarning("User not found Register first");
            return null;
        }
        if (user.EmailConfirmed == false)
        {
            _logger.LogWarning("Email not confirmed");
            return null;
        }
        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!result.Succeeded)
        {
            _logger.LogWarning("Invalid password");
            return null;
        }
        return user;
    }
    
    private async Task VerifyUserEmail(User user)
    {
        user.EmailConfirmed = true;
        await _userManager.UpdateAsync(user);
    }
    
    private async Task<bool> SendVerificationEmail(SendEmailDto dto)
    {
        var token = _authService.GenerateAccessTokenString(await _userManager.FindByEmailAsync(dto.Email));
        var url = Url.Action("VerifyEmail", "Auth", new { token }, Request.Scheme);
        var body = $"Please click the link to verify your email <a href='{url}'>Verify Email</a>";
        return await _emailService.SendEmail(dto.Email, "Email Verification", body);
    }

    private async Task<LoginResponse?> GenerateTokensAndUpdateUser(User user)
    {
        var accessToken = _authService.GenerateAccessTokenString(user);
        var refreshToken = _authService.GenerateRefreshTokenString();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        var check = await _userManager.UpdateAsync(user);
        if (!check.Succeeded)
        {
            _logger.LogError("An error occurred while updating the user");
            return null;
        }

        LoginResponse response = new()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
        return response;
    }

    
    
    /// //////////////////////////////////////////////////////////////////////////////////////////////
    
    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (await _userManager.FindByEmailAsync(dto.Email) is not null)
            return BadRequest("Email already exists ");
        
        if (await _userManager.FindByNameAsync(dto.UserName) is not null)
            return BadRequest("Username already exists ");

        var user = new User
        {
            UserName = dto.UserName,
            Email = dto.Email,
            EmailConfirmed = false,
            PhoneNumber = dto.PhoneNumber,
            PhoneNumberConfirmed = true
        };
        
        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);
        await _userManager.AddToRoleAsync(user, "User");

        var sendEmailDto = new SendEmailDto
        {
            Id = user.Id,
            Email = user.Email
        };
        
        var check = await SendVerificationEmail(sendEmailDto);
        if (!check)
            return BadRequest("An error occurred while sending the email please try again");
        return Ok("Check your email for verification");

    }
    
    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet("VerifyEmail/{token}")]
    public async Task<IActionResult> VerifyEmail([FromRoute]string token)
    {
        // var userId = _authService.GetUserIdFromToken(token);
        // if (userId is null)
        //     return BadRequest("Invalid token");
        // var user = await _userManager.FindByIdAsync(userId);
        // if (user is null)
        //     return BadRequest("User not found");
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        // _logger.LogInformation("userId: " + userId);
        if (userId is null)
            return BadRequest("Invalid token");
        
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return BadRequest("User not found");

        await VerifyUserEmail(user);
        return Ok("Email confirmed successfully");
    }
    
    
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var checkUser = await ValidateUser(dto);
        if (checkUser is null)
            return BadRequest("Email is not found register first or email not confirmed or invalid password");
        var response = await GenerateTokensAndUpdateUser(checkUser);
        if (response == null)
            return BadRequest("An error occurred while updating the user please try again");

        return Ok(response);
    }
    
    [HttpDelete("delete/{id}")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> DeleteUser([FromRoute] string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            return NotFound("User not found");
        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
            return BadRequest(result.Errors);
        return Ok("User deleted successfully");
    }
    
    
    [Authorize(Policy = "AdminOrUser")]
    [HttpPatch("changepassword")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        // var preToken = Request.Headers.Authorization.ToString();
        // //let's remove the part "Bearer " from the token
        // var token = preToken[7..];
        //
        // var userId = _authService.GetUserIdFromToken(token);
        // if (userId is null)
        //     return BadRequest("Invalid token");
        //
        // var user = await _userManager.FindByIdAsync(userId);
        // if (user is null)
        //     return BadRequest("User not found");
        
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        // _logger.LogInformation("userId: " + userId);
        if (userId is null)
            return BadRequest("Invalid token");
        
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return BadRequest("User not found");

        var result = await _userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);
        if (!result.Succeeded)
            return BadRequest(result.Errors);
        return Ok("Password changed successfully");
    }

    
    
    [HttpPost("RefreshToken")]
    [Authorize(Policy = "AdminOrUser")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto model)
    {
        // var userId = _authService.GetUserIdFromToken(model.AccessToken);
        // if (userId is null)
        //     return BadRequest("Invalid token");
        // var user = await _userManager.FindByIdAsync(userId);
        // if (user is null)
        //     return BadRequest("User not found");
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        // _logger.LogInformation("userId: " + userId);
        if (userId is null)
            return BadRequest("Invalid token");
        
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return BadRequest("User not found");



        var response = await GenerateTokensAndUpdateUser(user);
        if (response == null)
            return BadRequest("An error occurred while updating the user please try again");

        return Ok(response);
    }
    
    
    [HttpGet("softDelete/{id}")]
    [Authorize(Policy = "AdminOrUser")]
    public async Task<IActionResult> SoftDelete()
    {
        // var user = await _userManager.FindByIdAsync(id);
        // if (user is null)
        //     return NotFound("User not found");
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        // _logger.LogInformation("userId: " + userId);
        if (userId is null)
            return BadRequest("Invalid token");
        
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return BadRequest("User not found");

        user.IsDeleted = true;
        user.UndoIsDeletedCode = _authService.GenerateUndoSoftDeleteCode();
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return BadRequest(result.Errors);
        return Ok("User soft deleted successfully");
    }
    
    [HttpGet("activate/{id}")]
    public async Task<IActionResult> Activate([FromRoute] string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            return NotFound("User not found");
        if (user.IsDeleted == false)
            return BadRequest("User is already activated so login normally");
        var check = await _smsService.Send(user.PhoneNumber, user.UndoIsDeletedCode);
        if (check is null)
            return BadRequest("An error occurred while sending the code please try again");
        return Ok("Code sent successfully");
    }
    

    [HttpGet("activate/{id}/{code}")]
    [Authorize(Policy = "Admin,User")]
    public async Task<IActionResult> Activate([FromRoute] string id, [FromRoute] string code)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            return NotFound("User not found");
        if (user.IsDeleted == false)
            return BadRequest("User is already activated so login normally");
        if (user.UndoIsDeletedCode != code)
            return BadRequest("Invalid code");
        user.IsDeleted = false;
        user.UndoIsDeletedCode = null;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return BadRequest(result.Errors);
        return Ok("User activated successfully");
    }


    





}