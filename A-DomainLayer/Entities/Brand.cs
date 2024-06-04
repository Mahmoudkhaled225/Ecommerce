using System.Text.Json.Serialization;

namespace A_DomainLayer.Entities;

public class Brand : BaseEntity
{ 
    public string Name { get; set; }
    
    public string? ImgUrl { get; set; }
    public string? PublicId { get; set; }

    [JsonIgnore]
    public ICollection<Product> Products { get; set; }
}