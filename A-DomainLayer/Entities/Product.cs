using System.Text.Json.Serialization;

namespace A_DomainLayer.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; }
    public int BrandId { get; set; }
    [JsonIgnore]
    public Brand Brand { get; set; } = null!;
    
    public decimal Price { get; set; } 
}