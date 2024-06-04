using A_DomainLayer.Entities;
using A_DomainLayer.Interfaces;
using B_RepositoryLayer.Data;
using Microsoft.EntityFrameworkCore;

namespace B_RepositoryLayer.Repositories;

public class BrandRepository : GenericRepository<Brand>, IBrandRepository
{
    public BrandRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Brand?> GetWithInclude(int? id) =>
        await Context.Brands.Include(b => b.Products).FirstOrDefaultAsync(b => b.Id == id);
}