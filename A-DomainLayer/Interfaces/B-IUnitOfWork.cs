using A_DomainLayer.Entities;

namespace A_DomainLayer.Interfaces;

public interface IUnitOfWork : IAsyncDisposable
{
    
    IBrandRepository BrandRepository { get; set; }
    IProductRepository ProductRepository { get; set; }
    
    IOrderRepository OrderRepository { get; set; }
    IOrderItemRepository OrderItemRepository { get; set; }
    // ICartRepository CartRepository { get; set; }
    
    
    IDeliveryMethodRepository DeliveryMethodRepository { get; set; }
    

    
    Task<int> Save();
}