using A_DomainLayer.Entities;

namespace A_DomainLayer.Interfaces;

public interface IGenericRepository<T> where T : BaseEntity
{ 
    Task Add(T entity);
    void Delete(T entity);
    void Update(T entity);
    
    
    Task<T?> Get(int? id);
    Task<T?> GetWithSpec(ISpecification<T> spec);
    
    
    Task<IEnumerable<T?>> GetAll(); 
    Task<IEnumerable<T?>> GetAllWithSpec(ISpecification<T> spec);

    
    Task<IEnumerable<T?>> GetAllFunc(Func<T, bool> match);

}