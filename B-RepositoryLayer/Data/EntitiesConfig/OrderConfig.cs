using A_DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace B_RepositoryLayer.Data.EntitiesConfig;

public class OrderConfig : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id); // Set primary key

        
        builder.Property(o => o.UserId).IsRequired(); // Set UserId as required

        builder.Property(o => o.ShippingAddress).IsRequired(); // Set ShippingAddress as required
        
        builder.Property(o => o.Status)
            .HasConversion(
                o => o.ToString(),
                o => (OrderStatus)Enum.Parse(typeof(OrderStatus), o))  // Convert OrderStatus to string
            .IsRequired();

        builder.Property(o => o.PaymentMethod)
            .HasConversion(
                o => o.ToString(),
                o => (PaymentMethod)Enum.Parse(typeof(PaymentMethod), o)) // Convert PaymentMethod to string
            .IsRequired();
        
        builder.Property(o => o.Subtotal).HasColumnType("decimal(18,2)").IsRequired(); // Set precision for Subtotal

        // builder.Property(o => o.TotalPrice).HasColumnType("decimal(18,2)").IsRequired(); // Set precision for TotalPrice
        // builder.Property(o => o.TotalPrice)
        //     .HasColumnType("decimal(18,2)")
        //     .HasComputedColumnSql("[Subtotal] + [DeliveryMethod.Price]");

        
        // builder.HasMany(o => o.OrderItems) // One-to-many relationship with OrderItem
        //     .WithOne()
        //     .HasForeignKey(oi => oi.Id)
        //     .OnDelete(DeleteBehavior.Cascade); // Delete OrderItems when related Order is deleted
        
        builder.HasOne<OrderItem>()
            .WithOne()
            .HasForeignKey<Order>(o => o.OrderItemId)
            .OnDelete(DeleteBehavior.Cascade);
            

        // builder.HasOne(o => o.DeliveryMethod) // One-to-one relationship with DeliveryMethod
        //     .WithOne()
        //     .HasForeignKey<Order>(o => o.Id);
        // .OnDelete(DeleteBehavior.SetNull); // Set DeliveryMethod to null when related Order is deleted


    }
    
}

