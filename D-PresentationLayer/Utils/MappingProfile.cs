using A_DomainLayer.Entities;
using AutoMapper;
using D_PresentationLayer.Dtos.AuthDtos;
using D_PresentationLayer.Dtos.BrandDtos;
using D_PresentationLayer.Dtos.DeliveryMethodDtos;
using D_PresentationLayer.Dtos.ProductDto;

namespace D_PresentationLayer.Utils;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        
        CreateMap<Brand, ReturnBrand>().ReverseMap();
        CreateMap<Brand, ReturnedBrandForProduct>().ReverseMap();
        

        CreateMap<Product, ReturnProduct>().ReverseMap();
        CreateMap<Product, returnedProductForBrand>().ReverseMap();
        
        CreateMap<User, RegisterDto>().ReverseMap();



        CreateMap<DeliveryMethod, AddDeliveryMethod>().ReverseMap();
        CreateMap<DeliveryMethod, ReturnDeliveryMethod>().ReverseMap();
        // CreateMap<DeliveryMethod, UpdateDeliveryMethod>().ReverseMap();
        CreateMap<UpdateDeliveryMethod, DeliveryMethod>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        

    }

}