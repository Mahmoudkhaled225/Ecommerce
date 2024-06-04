using A_DomainLayer.Entities;
using A_DomainLayer.Interfaces;
using B_RepositoryLayer.Data;

namespace B_RepositoryLayer.Repositories;

public class DeliveryMethodRepository : GenericRepository<DeliveryMethod>, IDeliveryMethodRepository
{
    public DeliveryMethodRepository(ApplicationDbContext context) : base(context) { }
    
}