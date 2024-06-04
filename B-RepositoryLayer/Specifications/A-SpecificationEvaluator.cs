using A_DomainLayer.Entities;
using A_DomainLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace B_RepositoryLayer.Specifications;

// public class ParameterExpressionReplacer : ExpressionVisitor
// {
//     private readonly IReadOnlyList<ParameterExpression> _from, _to;
//
//     public ParameterExpressionReplacer(IReadOnlyList<ParameterExpression> from, IReadOnlyList<ParameterExpression> to)
//     {
//         if (from.Count != to.Count) throw new ArgumentException("Parameter lists must be the same length");
//         this._from = from;
//         this._to = to;
//     }
//
//     protected override Expression VisitParameter(ParameterExpression node)
//     {
//         for (int i = 0; i < _from.Count; i++)
//         {
//             if (node == _from[i]) return _to[i];
//         }
//         return node;
//     }
// }
// Modify the IQueryable using the specification's criteria expression
// if (specification.Criteria is not null && specification.Filter is not null)
// {
//     var combinedExpression = Expression.Lambda<Func<T, bool>>(
//         Expression.AndAlso(
//             specification.Criteria.Body,
//             new ParameterExpressionReplacer(specification.Criteria.Parameters, specification.Filter.Parameters).Visit(specification.Filter.Body)
//         ),
//         specification.Criteria.Parameters
//     );
//
//     query = query.Where(combinedExpression);
// }
public static class SpecificationEvaluator<T> where T : BaseEntity
{
    public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
    {
        var query = inputQuery;


        // Includes all expression-based includes
        query = specification.Includes.Aggregate(query,
            (current, include) => current.Include(include));
        // Include any string-based include statements
        query = specification.IncludeStrings.Aggregate(query,
            (current, include) => current.Include(include));

        
        
        // modify the IQueryable using the specification's criteria expression
        if (specification.Criteria is not null)
            query = query.Where(specification.Criteria);

        // Apply ordering if expressions are set
        if (specification.OrderBy is not null)
            query = query.OrderBy(specification.OrderBy);
        
        if (specification.OrderByDescending is not null)
            query = query.OrderByDescending(specification.OrderByDescending);
        
        if (specification.GroupBy is not null)
            query = query.GroupBy(specification.GroupBy).SelectMany(x => x);
        
        // Apply paging if enabled
        if (specification.IsPagingEnabled)
            query = query.Skip(specification.Skip.Value).Take(specification.Take.Value);
        


        
        return query;

    }
}