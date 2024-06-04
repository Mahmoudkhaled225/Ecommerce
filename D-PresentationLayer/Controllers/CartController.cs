using System.Security.Claims;
using A_DomainLayer.Entities;
using A_DomainLayer.Interfaces;
using AutoMapper;
using C_ServiceLayer.Abstractions;
using D_PresentationLayer.Dtos.CartDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace D_PresentationLayer.Controllers;

[Authorize(Policy = "AdminOrUser")]
public class CartController : BaseController
{
    private readonly IMapper _mapper;
    private readonly ILogger _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthService _authService;
    private readonly UserManager<User> _userManager;
    private readonly ICartRepository _cartRepository;
    
    public CartController(IMapper mapper, ILogger<CartController> logger, IUnitOfWork unitOfWork, 
        IAuthService authService, UserManager<User> userManager, ICartRepository cartRepository)
    {
        _mapper = mapper;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _authService = authService;
        _userManager = userManager;
        _cartRepository = cartRepository;
    }    
    
    
    private async Task<User?> GetUserFromToken(string token)
    {
        var userId = _authService.GetUserIdFromToken(token);
        if (userId is null)
            return null;
        var user = _userManager.FindByIdAsync(userId).Result;
        if (user is null)
            return null;
        return user;
    }
    
    [HttpGet("Get")]
    public async Task<IActionResult> GetCart()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId is null)
            return BadRequest("Invalid token");
        
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return BadRequest("User not found");
        
        var cart = await _cartRepository.GetCart(user.Id);
        if (cart is null)
            cart = await _cartRepository.CreateCart(user.Id);
        return Ok(cart);
    }

    [HttpPost("AddToCart")]
    public async Task<ActionResult<Cart>> AddToCart([FromBody] AddToCartDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId is null)
            return BadRequest("Invalid token");
        
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return BadRequest("User not found");
        
        var product = await _unitOfWork.ProductRepository.Get(dto.ProductId);
        if (product is null)
            return NotFound("Product not found");

        var cart = await _cartRepository.GetCart(user.Id);
        if (cart is null)
            cart = await _cartRepository.CreateCart(user.Id);
        
        var x = await _cartRepository.AddToCart(user.Id, product.Id, dto.Quantity ?? 1);
        return Ok(x);
    }
    
    [HttpDelete("RemoveFromCart")]
    public async Task<ActionResult<Cart>> RemoveFromCart([FromBody] int productId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId is null)
            return BadRequest("Invalid token");
        
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return BadRequest("User not found");

        var product = await _unitOfWork.ProductRepository.Get(productId);
        if (product is null)
            return NotFound("Product not found");

        var cart = await _cartRepository.GetCart(user.Id);
        if (cart is null)
            return NotFound("Cart not found");

        var x = await _cartRepository.RemoveFromCart(user.Id, product.Id);
        return Ok(x);
    }
}