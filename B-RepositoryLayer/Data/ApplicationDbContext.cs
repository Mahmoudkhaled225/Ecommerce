using System.Reflection;
using A_DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace B_RepositoryLayer.Data;

public class ApplicationDbContext : DbContext
{
    
    public ApplicationDbContext() { }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly()); 
    

    
    public DbSet<Brand> Brands { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    
    public DbSet<DeliveryMethod> DeliveryMethods { get; set; } = null!;
    
    public DbSet<Order> Orders { get; set; } = null!;
    
    public DbSet<OrderItem> OrderItems { get; set; } = null!;
    
    
    

}