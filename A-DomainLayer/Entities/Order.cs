using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace A_DomainLayer.Entities;


public class Address
{
    public string ApartmentNum { get; set; }
    public string BuildingNum { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string Country { get; set; } = "egypt";
}

public class ProductToOrderItem: BaseEntity
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
public class OrderItem : BaseEntity
{ 
    
    public List<ProductToOrderItem> Products { get; set; } = new List<ProductToOrderItem>();
    public string UserId { get; set; }
}
public enum OrderStatus
{
    [Description("pending")]
    Pending,

    [Description("confirmed")]
    Confirmed,

    [Description("placed")]
    Placed,

    [Description("on way")]
    OnWay,

    [Description("delivered")]
    Delivered,

    [Description("cancelled")]
    Cancelled,

    [Description("rejected")]
    Rejected
}

public enum PaymentMethod
{
    Cash,
    Card
}


public class DeliveryMethod : BaseEntity
{
    public string Name { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; } 
    public string DeliveryTime { get; set; }

}
 

public class Order : BaseEntity
{
    public string UserId { get; set; } /// ====> also to get his phone
    
    /// more accurate address
    /// public Address ShippingAddress { get; set; }   
    /// for simplicity I will use string
    public string ShippingAddress { get; set; }
    
    // public ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>(); 
    // public List<OrderItem> OrderItems { get; set; }  

    public int OrderItemId { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    
    
    public int DeliveryMethodId { get; set; }
    public DeliveryMethod DeliveryMethod { get; set; }

    public PaymentMethod PaymentMethod { get; set; }

    public decimal Subtotal { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public decimal TotalPrice => Subtotal+DeliveryMethod.Price;
    
    // public decimal TotalPrice
    // {
    //     get
    //     {
    //         return Subtotal + DeliveryMethod.Price;
    //     }
    //     private set { } // Add a private setter
    // }

    
    
    //////////////////////////////////////////////////////////
    public string? PaymentIntentId { get; set; } 

}