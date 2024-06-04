namespace D_PresentationLayer.Dtos.BrandDtos;

public class ReturnBrand
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? ImgUrl { get; set; }
    public ICollection<returnedProductForBrand> Products { get; set; }

}