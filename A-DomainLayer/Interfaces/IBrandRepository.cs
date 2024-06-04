using A_DomainLayer.Entities;

namespace A_DomainLayer.Interfaces;

public interface IBrandRepository : IGenericRepository<Brand>
{
    Task<Brand?> GetWithInclude(int? id);
}