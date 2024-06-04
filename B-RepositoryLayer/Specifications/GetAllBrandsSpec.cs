using System.Linq.Expressions;
using A_DomainLayer.Entities;
using A_DomainLayer.Specifications;

namespace B_RepositoryLayer.Specifications;

public class GetAllBrandsSpec : BaseSpecification<Brand>
{

    public GetAllBrandsSpec(int? pageNumber, int? pageSize)
    {
        Includes.Add(b => b.Products);
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        AddOrderBy(b => b.Name);
    }
    
    public GetAllBrandsSpec(int id) : base(b => b.Id == id)
    {
        Includes.Add(b => b.Products);
        AddOrderBy(b => b.Name);
    }

    public GetAllBrandsSpec(string name, int? pageNumber, int? pageSize) : base(b => b.Name == name)
    {
        Includes.Add(b => b.Products);
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        AddOrderBy(b => b.Name);
    }

    public GetAllBrandsSpec(Expression<Func<Brand, bool>> criteria) : base(criteria)
    {
        Includes.Add(b => b.Products);
        AddOrderBy(b => b.Name);
    }
    
    public GetAllBrandsSpec(string name) : base(b => b.Name == name)
    {
        Includes.Add(b => b.Products);
    }
    
    public GetAllBrandsSpec()
    {
        Includes.Add(b => b.Products);
    }
    
    
}