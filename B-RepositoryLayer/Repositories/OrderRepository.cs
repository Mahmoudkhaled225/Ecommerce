using A_DomainLayer.Entities;
using A_DomainLayer.Interfaces;
using B_RepositoryLayer.Data;

namespace B_RepositoryLayer.Repositories;

public class OrderRepository: GenericRepository<Order>, IOrderRepository
{
    
    public OrderRepository(ApplicationDbContext context) : base(context) { }

    
}