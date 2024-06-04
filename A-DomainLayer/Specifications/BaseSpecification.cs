using System.Linq.Expressions;
using A_DomainLayer.Entities;
using A_DomainLayer.Interfaces;

namespace A_DomainLayer.Specifications;

public abstract class  BaseSpecification<T> : ISpecification<T> where T : BaseEntity
{
    public Expression<Func<T, bool>>? Criteria { get; set; }
    public Expression<Func<T, bool>>? Filter { get; set; }
    
    
    public List<Expression<Func<T, object>>> Includes { get; set; } = new List<Expression<Func<T, object>>>();
    public List<string> IncludeStrings { get; set; } = new List<string>();

    
    public Expression<Func<T, object>>? OrderBy { get; set; } 
    public Expression<Func<T, object>>? OrderByDescending { get; set; }
    
    
    public Expression<Func<T, object>>? GroupBy { get; set; }
    
    
    public int? Take { get; set; }
    public int? Skip { get; set; }
    public bool IsPagingEnabled { get; set;  } = false;


    protected BaseSpecification() { }
    
    protected BaseSpecification(Expression<Func<T, bool>>? criteria) =>
        Criteria = criteria;
    
    
    protected virtual void AddOrderBy(Expression<Func<T, object>> orderByExpression) =>
        OrderBy = orderByExpression;
    

    protected virtual void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression) =>
        OrderByDescending = orderByDescendingExpression;
    
    protected virtual void ApplyGroupBy(Expression<Func<T, object>> groupByExpression) =>
        GroupBy = groupByExpression;
    
    protected virtual void ApplyPaging(int? skip, int ?take)
    {
        Skip = skip ?? 0;
        Take = take ?? 10;
        IsPagingEnabled = true;
    }
    
}
