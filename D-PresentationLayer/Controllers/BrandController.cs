using A_DomainLayer.Entities;
using A_DomainLayer.Interfaces;
using AutoMapper;
using B_RepositoryLayer.Specifications;
using C_ServiceLayer.Abstractions;
using D_PresentationLayer.Dtos.BrandDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace D_PresentationLayer.Controllers;

public class BrandController : BaseController
{
    
    private readonly IMapper _mapper;
    private readonly ILogger _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUploadImgService _uploadImgService;



    public BrandController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<BrandController> logger, IUploadImgService uploadImgService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _uploadImgService = uploadImgService;
    }
    
    
    
    private async Task<(string? imgUrl, string? publicId)> UploadUserImage(IFormFile? image)
    {
        if (image == null)
            return (null, null);
        var uploadResult = await _uploadImgService.UploadImg(image);
        return uploadResult != null ? (uploadResult.Url.ToString(), uploadResult.PublicId) : (null, null);
    }

    [HttpPost("Add")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> AddBrand([FromForm]AddBrand dto)
    {
        // first we need to check if the brand already exists by name
        var brandExists = await _unitOfWork.BrandRepository.GetWithSpec(new GetAllBrandsSpec(dto.Name));
        if (brandExists != null)
            return BadRequest("Brand with this name already exists");
        var (imgUrl, publicId) = await UploadUserImage(dto.Image);
        var brand = new Brand
        {
            Name = dto.Name,
            ImgUrl = imgUrl,
            PublicId = publicId
        };

        await _unitOfWork.BrandRepository.Add(brand);
        var flag = await _unitOfWork.Save();
        if (flag is 1)
            return Created("Brand has been added", _mapper.Map<ReturnBrand>(brand));
        
        _logger.LogError("Error in saving brand");
        return BadRequest("Error in saving brand");
    }
    
    [HttpGet("GetAll")]
    [Authorize(Policy = "AdminOrUser")]
    public async Task<IActionResult> GetBrands([FromQuery] int? pageNumber, [FromQuery] int? pageSize)
    {
        var brands = await _unitOfWork.BrandRepository.GetAllWithSpec(new GetAllBrandsSpec(pageNumber, pageSize));
        if (brands is null)
            return NotFound("No brands found");
        return Ok(_mapper.Map<IEnumerable<ReturnBrand>>(brands));
    }

    [HttpGet("Get/{id:int}")]   
    [Authorize(Policy = "AdminOrUser")]
    public async Task<IActionResult> GetBrand([FromRoute]int id)
    {
        var brand = await _unitOfWork.BrandRepository.GetWithSpec(new GetAllBrandsSpec(id));
        if (brand is null)
            return NotFound("Brand not found");
        return Ok(_mapper.Map<ReturnBrand>(brand));
    }
    
    
}