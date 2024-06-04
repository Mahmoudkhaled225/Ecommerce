namespace D_PresentationLayer.Dtos.ProductDto;

public class ReturnProduct
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public decimal Price { get; set; }
    
    public ReturnedBrandForProduct Brand { get; set; }

}