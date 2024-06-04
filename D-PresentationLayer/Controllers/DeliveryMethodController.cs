using A_DomainLayer.Entities;
using A_DomainLayer.Interfaces;
using AutoMapper;
using D_PresentationLayer.Dtos.DeliveryMethodDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace D_PresentationLayer.Controllers;


[Authorize(Policy = "Admin")]
public class DeliveryMethodController : BaseController
{
    
    private readonly IMapper _mapper;
    private readonly ILogger _logger;
    private readonly IUnitOfWork _unitOfWork;
    
    public DeliveryMethodController(IMapper mapper, ILogger<DeliveryMethodController> logger, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    
    [HttpPost("Add")]
    public async Task<IActionResult> AddDeliveryMethod([FromBody]AddDeliveryMethod dto)
    {
        var deliveryMethod = new DeliveryMethod
        {
            Name = dto.Name,
            Price = dto.Price,
            DeliveryTime = dto.DeliveryTime
        };
        await _unitOfWork.DeliveryMethodRepository.Add(deliveryMethod);
        var flag = await _unitOfWork.Save();
        if (flag is 1)
            return Created("Delivery method has been added", _mapper.Map<ReturnDeliveryMethod>(deliveryMethod));
        return BadRequest("Failed to add delivery method");
    }
    
    [HttpPatch("Update/{id:int}")]
    public async Task<IActionResult> UpdateDeliveryMethod([FromRoute] int id, [FromBody]UpdateDeliveryMethod dto)
    {
        var deliveryMethod = await _unitOfWork.DeliveryMethodRepository.Get(id);
        if (deliveryMethod is null)
            return NotFound("Delivery method not found");
        if (dto.Name != null)
            deliveryMethod.Name = dto.Name;
        if (dto.Price != null)
            deliveryMethod.Price = dto.Price.Value;
        if (dto.DeliveryTime != null)
            deliveryMethod.DeliveryTime = dto.DeliveryTime;
        // _mapper.Map(dto, deliveryMethod);

        
        _unitOfWork.DeliveryMethodRepository.Update(deliveryMethod);
        var flag = await _unitOfWork.Save();
        if (flag is 1)
            return Ok(_mapper.Map<ReturnDeliveryMethod>(deliveryMethod));
        return BadRequest("Failed to update delivery method");
    }
    
    [HttpDelete("Delete")]
    public async Task<IActionResult> DeleteDeliveryMethod([FromRoute] int id)
    {
        var deliveryMethod = await _unitOfWork.DeliveryMethodRepository.Get(id);
        if (deliveryMethod is null)
            return NotFound("Delivery method not found");
        _unitOfWork.DeliveryMethodRepository.Delete(deliveryMethod);
        var flag = await _unitOfWork.Save();
        if (flag is 1)
            return Ok("Delivery method has been deleted");
        return BadRequest("Failed to delete delivery method");
    }
    
    [AllowAnonymous]    
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetDeliveryMethods()
    {
        var deliveryMethods = await _unitOfWork.DeliveryMethodRepository.GetAll();
        if (deliveryMethods is null)
            return NotFound("No delivery methods found");
        return Ok(_mapper.Map<IEnumerable<ReturnDeliveryMethod>>(deliveryMethods));
    }
    
    
}