using A_DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace B_RepositoryLayer.Data.EntitiesConfig;

public class ProductConfig : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id); // Set primary key

        builder.Property(p => p.Name).IsRequired(); // Set Name as required


        builder.Property(p => p.Price).HasColumnType("decimal(18,2)").IsRequired(); // Set precision for Price
    }
    
}