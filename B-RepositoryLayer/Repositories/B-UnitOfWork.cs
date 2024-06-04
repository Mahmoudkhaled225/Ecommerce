using System.Collections;
using A_DomainLayer.Entities;
using A_DomainLayer.Interfaces;
using B_RepositoryLayer.Data;

namespace B_RepositoryLayer.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context; 
    
    public IBrandRepository BrandRepository { get; set; } 
    public IProductRepository ProductRepository { get; set; }
    
    public IOrderRepository OrderRepository { get; set; }
    public IOrderItemRepository OrderItemRepository { get; set; }
    public IDeliveryMethodRepository DeliveryMethodRepository { get; set; }

    // public ICartRepository CartRepository { get; set; }

    
    public UnitOfWork(ApplicationDbContext context, 
        IBrandRepository brandRepository, 
        IProductRepository productRepository, 
        IOrderRepository orderRepository, 
        IOrderItemRepository orderItemRepository, 
        IDeliveryMethodRepository deliveryMethodRepository)
    {
        _context = context;
        BrandRepository = brandRepository;
        ProductRepository = productRepository;
        OrderRepository = orderRepository;
        OrderItemRepository = orderItemRepository;
        DeliveryMethodRepository = deliveryMethodRepository;
    }
    
    

    
    
    
    
    public async Task<int> Save() => 
        await _context.SaveChangesAsync();
    
    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
        // for better performance, 
        // use it when your class might be inherited by other classes that could introduce a finalizer
        // This will prevent the garbage collector from calling the finalizer of the UnitOfWork object, if one exists
        GC.SuppressFinalize(this);

    }
}
