using System.Text.Json;
using A_DomainLayer.Entities;
using A_DomainLayer.Interfaces;
using StackExchange.Redis;

namespace B_RepositoryLayer.Repositories;

public class CartRepository : ICartRepository
{
    private readonly IDatabase _database;

    public CartRepository(IConnectionMultiplexer redis) =>
        _database = redis.GetDatabase();

    public async Task<Cart?> CreateCart(string id)
    {
        var cart = new Cart(id);
        await _database.StringSetAsync(id, JsonSerializer.Serialize(cart));
        return cart;
    }
    
    
    public async Task<Cart?> GetCart(string? id)
    {
        var data = await _database.StringGetAsync(id);
        return data.IsNull ? null : JsonSerializer.Deserialize<Cart>(data);
    }    


    // public async Task<Cart> AddToCart(string? userId, Product product)
    // {
    //     var cart = await GetCart(userId);
    //     if (cart == null)
    //     {
    //         cart = new Cart(userId);
    //         cart.ProductsList.Add(product);
    //     }
    //     else 
    //         cart?.ProductsList.Add(product);
    //     
    //     await _database.StringSetAsync(userId, JsonSerializer.Serialize(cart));
    //     return cart;
    // }
    // public async Task<Cart?> RemoveFromCart(string? id, Product product) 
    // {
    //     var cart = await GetCart(id);
    //     if (cart == null)
    //         return null;
    //
    //     var productToRemove = cart.ProductsList.FirstOrDefault(p => p.Id == product.Id);
    //     
    //     if (productToRemove is null)
    //         return cart;
    //
    //     cart.ProductsList.Remove(productToRemove);
    //     await _database.StringSetAsync(id, JsonSerializer.Serialize(cart));
    //
    //     return cart;
    // }
    //
    // public async Task<Cart?> ClearCart(string? id) 
    // {
    //     var cart = await GetCart(id);
    //     if (cart == null)
    //         return null;
    //
    //     cart.ProductsList.Clear();
    //     await _database.StringSetAsync(id, JsonSerializer.Serialize(cart));
    //
    //     return cart;
    // }
    
    public async Task<Cart> AddToCart(string? userId, int productId, int quantity = 1)  
    {
        var cart = await GetCart(userId);
        if (cart == null)
        {
            cart = new Cart(userId);
            cart.CartItems.Add(new CartItem { ProductId = productId, Quantity = quantity });
        }
        else
        {
            var cartItem = cart.CartItems.FirstOrDefault(i => i.ProductId == productId);
            if (cartItem == null)
            {
                cart.CartItems.Add(new CartItem { ProductId = productId, Quantity = quantity });
            }
            else
            {
                cartItem.Quantity += quantity;
            }
        }

        await _database.StringSetAsync(userId, JsonSerializer.Serialize(cart));
        return cart;
    }


    public async Task<Cart?> RemoveFromCart(string? userId, int productId)
    {
        var cart = await GetCart(userId);
        if (cart == null)
            return null;

        var cartItem = cart.CartItems.FirstOrDefault(i => i.ProductId == productId);
        if (cartItem == null)
            return cart;

        if (cartItem.Quantity > 1)
        {
            cartItem.Quantity--;
        }
        else
        {
            cart.CartItems.Remove(cartItem);
        }

        await _database.StringSetAsync(userId, JsonSerializer.Serialize(cart));
        return cart;
    }

    public async Task<Cart?> ClearCart(Cart cart, string userId)
    {

        cart.CartItems.Clear();
        await _database.StringSetAsync(userId, JsonSerializer.Serialize(cart));

        return cart;
    }

    public async Task<bool> UpdateCart(Cart cart) =>
        await _database.StringSetAsync(cart.UserId, JsonSerializer.Serialize(cart), TimeSpan.FromMinutes(5));

    public async Task<bool> DeleteCart(string? id) => 
        await _database.KeyDeleteAsync(id);

}