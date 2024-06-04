using A_DomainLayer.Entities;
using A_DomainLayer.Interfaces;
using B_RepositoryLayer.Data;
using B_RepositoryLayer.Specifications;
using Microsoft.EntityFrameworkCore;

namespace B_RepositoryLayer.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    
    protected readonly ApplicationDbContext Context;

    protected GenericRepository(ApplicationDbContext context) =>
        Context = context;

    private IQueryable<T> ApplySpecification(ISpecification<T> spec) =>
        SpecificationEvaluator<T>.GetQuery(Context.Set<T>().AsQueryable(), spec);

    public async Task<T?> Get(int? id) =>
        await Context.Set<T>().FindAsync(id);

    public async Task<IEnumerable<T>> GetAll() =>
        await Context.Set<T>().ToListAsync();
    
    public async Task Add(T entity) =>
        await Context.Set<T>().AddAsync(entity);

    public void Delete(T entity) =>
        Context.Set<T>().Remove(entity);

    public void Update(T entity) =>
        Context.Set<T>().Update(entity);

    public async Task<T?> GetWithSpec(ISpecification<T> spec /*= null*/) =>
        await this.ApplySpecification(spec).FirstOrDefaultAsync();

    public async Task<IEnumerable<T?>> GetAllWithSpec(ISpecification<T> spec) =>
        await this.ApplySpecification(spec).ToListAsync();

    public async Task<IEnumerable<T?>> GetAllFunc(Func<T, bool> match) =>
        throw new NotImplementedException();

}