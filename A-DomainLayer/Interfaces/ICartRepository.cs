using A_DomainLayer.Entities;

namespace A_DomainLayer.Interfaces;

public interface ICartRepository
{
    Task<Cart?> CreateCart(string? id);
    Task<Cart?> GetCart(string? id);
    Task<Cart> AddToCart(string? userId, int productId, int quantity = 1);
    Task<Cart?> RemoveFromCart(string? userId, int productId);
    Task<Cart?> ClearCart(Cart cart, string userId);
    Task<bool> UpdateCart(Cart cart);
    Task<bool> DeleteCart(string? id);
}