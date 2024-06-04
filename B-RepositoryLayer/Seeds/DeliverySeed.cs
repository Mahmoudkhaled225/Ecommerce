using A_DomainLayer.Entities;
using B_RepositoryLayer.Data;

namespace B_RepositoryLayer.Seeds;

public static class DeliverySeed
{
    public static async Task SeedDeliveryAsync(ApplicationDbContext context)
    {
        if (!context.DeliveryMethods.Any())
        {
            var deliveries = new List<DeliveryMethod>
            {
                new DeliveryMethod {Name = "Standard", DeliveryTime = "5 to 7 Days", Price = 10 },
                new DeliveryMethod {Name = "Express", DeliveryTime = "1 to 2 Days", Price = 20 },
            };
            await context.DeliveryMethods.AddRangeAsync(deliveries);
            await context.SaveChangesAsync();
        }
    }
}