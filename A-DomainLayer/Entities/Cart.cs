namespace A_DomainLayer.Entities;


public class CartItem
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
public class Cart
{
    public string UserId { get; set; }
    public List<CartItem> CartItems { get; set; }
    
    public Cart() { }
    public Cart(string? id)
    {
        UserId = id;
        CartItems = new List<CartItem>();
    }

    public Cart(CartItem item) =>
        CartItems?.Add(item); 
}
