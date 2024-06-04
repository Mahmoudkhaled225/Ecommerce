using A_DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace B_RepositoryLayer.Data.EntitiesConfig;

public class OrderItemConfig : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(oi => oi.Id); // Set primary key

        // builder.Property(oi => oi.Quantity).IsRequired(); // Set Quantity as required
        //
        // builder.Property(oi => oi.ProductId).IsRequired(); // Set ProductId as required
        
        
        
        builder.Property(oi => oi.UserId).IsRequired(); // Set UserId as required
        
    }
    
}