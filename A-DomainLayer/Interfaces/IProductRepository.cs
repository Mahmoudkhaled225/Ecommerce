using A_DomainLayer.Entities;

namespace A_DomainLayer.Interfaces;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<Product> GetWithInclude(int? id);
}