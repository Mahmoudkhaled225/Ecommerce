using A_DomainLayer.Entities;
using A_DomainLayer.Interfaces;
using B_RepositoryLayer.Data;

namespace B_RepositoryLayer.Repositories;

public class OrderItemRepository: GenericRepository<OrderItem>, IOrderItemRepository
{
    
    public OrderItemRepository(ApplicationDbContext context) : base(context) { }

    
}