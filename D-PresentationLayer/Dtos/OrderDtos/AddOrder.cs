using A_DomainLayer.Entities;

namespace D_PresentationLayer.Dtos.OrderDtos;

public class AddOrder
{
    public string ShippingAddress { get; set; }
    // public List<OrderItem>? OrderItems { get; set; }
    
    // public OrderStatus Status { get; set; }
    public int DeliveryMethodId { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
}