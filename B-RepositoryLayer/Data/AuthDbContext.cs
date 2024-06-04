using A_DomainLayer.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace B_RepositoryLayer.Data;

public class AuthDbContext : IdentityDbContext<User>
{
    
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.UserName)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
    
    // public DbSet<User> Users { get; set; } = null!;    
}