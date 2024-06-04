using A_DomainLayer.Entities;
using A_DomainLayer.Specifications;

namespace B_RepositoryLayer.Specifications;

public class GetAllProductsSpec : BaseSpecification<Product>
{
    public GetAllProductsSpec()
    {
        Includes.Add(p => p.Brand);
    }
    
    public GetAllProductsSpec(int id) : base(p => p.Id == id)
    {
        Includes.Add(p => p.Brand);
    }
}