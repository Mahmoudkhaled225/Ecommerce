using System.Linq.Expressions;
using A_DomainLayer.Entities;

namespace A_DomainLayer.Interfaces;

public interface ISpecification <T> where T : BaseEntity
{
    Expression<Func<T, bool>>? Criteria { get; set; }
    
    
    
    ///??
    Expression<Func<T, bool>>? Filter { get; set; }

    List<Expression<Func<T, object>>> Includes { get; set; }
    
    List<string> IncludeStrings { get; set; }
    
    Expression<Func<T, object>>? OrderBy { get; set; }
    Expression<Func<T, object>>? OrderByDescending { get; set; }
    
    
    Expression<Func<T,object>>? GroupBy { get; set; }
    
    int? Take { get; set; }
    int? Skip { get; set; }
    bool IsPagingEnabled { get; set; }

}
