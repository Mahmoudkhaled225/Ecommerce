using A_DomainLayer.Entities;
using A_DomainLayer.Interfaces;
using AutoMapper;
using B_RepositoryLayer.Specifications;
using D_PresentationLayer.Dtos.ProductDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace D_PresentationLayer.Controllers;

public class ProductController : BaseController
{
    
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    private readonly IUnitOfWork _unitOfWork;

    public ProductController(IMapper mapper, ILogger<ProductController> logger, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }    
    
    [HttpPost("Add")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> AddProduct([FromBody] AddProduct dto)
    {
        var product = new Product { Name = dto.Name, BrandId = dto.BrandId , Price = dto.Price};
        var checkBrand = await _unitOfWork.BrandRepository.Get(dto.BrandId);
        if (checkBrand is null)
            return NotFound("Brand not found so add brand first then add product");
        await _unitOfWork.ProductRepository.Add(product);
        var flag = await _unitOfWork.Save();
        if (flag is 1)
            return Created("Product has been added", _mapper.Map<ReturnProduct>(product));
        return BadRequest("Error in saving product");
    }
    
    
    [HttpGet("GetAll")]
    [Authorize(Policy = "AdminOrUser")]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _unitOfWork.ProductRepository.GetAllWithSpec(new GetAllProductsSpec());
        if (products is null)
            return NotFound("No brands found");
        return Ok(_mapper.Map<IEnumerable<ReturnProduct>>(products));
    }
    
    [HttpGet("Get/{id:int}")]
    [Authorize(Policy = "AdminOrUser")]
    public async Task<IActionResult> GetProduct([FromRoute] int id)
    {
        // var product = await _unitOfWork.ProductRepository.GetWithInclude(id);
        var product = await _unitOfWork.ProductRepository.GetWithSpec(new GetAllProductsSpec(id));
        if (product is null)
            return NotFound("Product not found");
        
        return Ok(_mapper.Map<ReturnProduct>(product));
    }







}