using A_DomainLayer.Entities;
using A_DomainLayer.Interfaces;
using B_RepositoryLayer.Data;
using Microsoft.EntityFrameworkCore;

namespace B_RepositoryLayer.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    
    public ProductRepository(ApplicationDbContext context) : base(context) { }
    
    public async Task<Product> GetWithInclude(int? id) =>
        await Context.Products.Include(p => p.Brand).FirstOrDefaultAsync(p => p.Id == id);
    
}