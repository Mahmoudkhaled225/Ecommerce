namespace D_PresentationLayer.Dtos.ProductDto;

public class AddProduct
{
    public string Name { get; set; }
    public int BrandId { get; set; }
    
    public decimal Price { get; set; }
}